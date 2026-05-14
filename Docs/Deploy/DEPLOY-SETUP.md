# Guia de Infraestrutura e Deploy — GDAC Platform

Documenta toda a configuração necessária para reproduzir do zero os ambientes de produção e staging dos serviços `Gdac.Auth.Api` e `Gdac.Core.Api`.

---

## Índice

1. [Visão geral](#1-visão-geral)
2. [Pré-requisitos](#2-pré-requisitos)
3. [Chaves RSA JWT](#3-chaves-rsa-jwt)
4. [VPS — provisionamento](#4-vps--provisionamento)
5. [VPS — arquivos .env](#5-vps--arquivos-env)
6. [GitHub — secrets e permissões](#6-github--secrets-e-permissões)
7. [Branches e fluxo de deploy](#7-branches-e-fluxo-de-deploy)
8. [Primeiro deploy manual](#8-primeiro-deploy-manual)
9. [Banco de dados](#9-banco-de-dados)
10. [E-mail (Serilog + SMTP KingHost)](#10-e-mail-serilog--smtp-kinghost)
11. [Referência rápida de ambientes](#11-referência-rápida-de-ambientes)

---

## 1. Visão geral

```
GitHub push
    └─> CI (build + test + docker push → ghcr.io)
            ├─> branch main    → Deploy Production
            └─> branch staging → Deploy Staging

VPS KingHost (Ubuntu 24.04)
    ├─ nginx (host) — SSL/TLS Let's Encrypt (certificado SAN)
    │
    ├─ /opt/gdac/auth/        → Auth prod  (porta interna 8080)
    ├─ /opt/gdac/auth-stg/    → Auth stg   (porta interna 8081)
    ├─ /opt/gdac/core/        → Core prod  (porta interna 8082)
    └─ /opt/gdac/core-stg/    → Core stg   (porta interna 8083)
```

**Serviços:**

| Serviço | Responsabilidade | Imagem Docker |
|---------|-----------------|---------------|
| `Gdac.Auth.Api` | Autenticação: registro, login, tokens JWT RS256 | `ghcr.io/<owner>/gdac-auth` |
| `Gdac.Core.Api` | Perfis de usuário e diretório de empresas | `ghcr.io/<owner>/gdac-core` |

**Stack:**
- Aplicação: .NET 10 / ASP.NET Core
- Banco: MariaDB 11.4 externo em `mysql.gdac.com.br`
- Cache: Redis 7.4-alpine (Auth apenas)
- JWT: RS256, chaves RSA 4096 bits — Auth emite, Core valida
- Container registry: GitHub Container Registry (`ghcr.io`)
- Proxy reverso: nginx no host
- SSL: Let's Encrypt (certbot), certificado SAN para todos os domínios

---

## 2. Pré-requisitos

### Local
- Git, GitHub CLI (`gh`)
- OpenSSL
- Python 3 (para gerar os `.env` com chaves JWT formatadas)
- Acesso SSH ao VPS

### VPS
- Ubuntu 24.04 LTS
- Acesso root via SSH
- Domínios DNS apontando para o IP do VPS **antes** de rodar o certbot:

| Registro | Tipo | Valor |
|----------|------|-------|
| `auth.gdac.com.br` | A | `<IP do VPS>` |
| `auth-stg.gdac.com.br` | A | `<IP do VPS>` |
| `core-api.gdac.com.br` | A | `<IP do VPS>` |
| `core-api-stg.gdac.com.br` | A | `<IP do VPS>` |

---

## 3. Chaves RSA JWT

As chaves são geradas **uma vez por par de ambientes** (prod e stg).  
O Auth usa o par completo (privada + pública). O Core usa **apenas a chave pública**.

```bash
# Produção
openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out jwt_prod_private.pem
openssl rsa -pubout -in jwt_prod_private.pem -out jwt_prod_public.pem

# Staging
openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out jwt_stg_private.pem
openssl rsa -pubout -in jwt_stg_private.pem -out jwt_stg_public.pem

# Converter para linha única (para colar no .env ou GitHub secrets)
awk 'NF {printf "%s\\n", $0}' jwt_prod_private.pem
awk 'NF {printf "%s\\n", $0}' jwt_prod_public.pem
```

> **Importante:** os arquivos `.env` e secrets do GitHub devem conter as chaves com `\n` **literal** (não quebras de linha reais). O código converte `\n` → newline em tempo de execução.

Guarde os arquivos `.pem` fora do repositório (gerenciador de senhas ou vault).

---

## 4. VPS — provisionamento

### 4.1 Acessar o servidor

```bash
ssh -i ~/.ssh/gdac_vps_key root@<IP_DO_VPS>
```

### 4.2 Executar o script de setup

```bash
# No VPS, como root
curl -fsSL https://raw.githubusercontent.com/<owner>/Gdac.Solutions.2026/main/Docs/Deploy/setup-vps.sh \
  | bash -s auth.gdac.com.br auth-stg.gdac.com.br suporte@gdac.com.br
```

O script instala Docker, nginx, certbot, UFW e cria `/opt/gdac/auth` e `/opt/gdac/auth-stg`.

### 4.3 Criar diretórios para o Core

```bash
# No VPS, como root
mkdir -p /opt/gdac/core /opt/gdac/core-stg

git clone https://github.com/<owner>/Gdac.Solutions.2026.git /opt/gdac/core
git clone https://github.com/<owner>/Gdac.Solutions.2026.git /opt/gdac/core-stg

# core-stg usa branch staging
cd /opt/gdac/core-stg && git checkout staging
```

### 4.4 Configurar nginx para Core

```bash
# Copiar configurações
cp /opt/gdac/core/docker/Core/nginx/conf.d/production.conf /etc/nginx/conf.d/core-production.conf
cp /opt/gdac/core/docker/Core/nginx/conf.d/staging.conf    /etc/nginx/conf.d/core-staging.conf

nginx -t && systemctl reload nginx
```

### 4.5 Expandir o certificado SSL

Se você já tem um certificado SAN para `auth.gdac.com.br`, expanda-o adicionando os novos domínios:

```bash
certbot --nginx \
  -d auth.gdac.com.br \
  -d auth-stg.gdac.com.br \
  -d core-api.gdac.com.br \
  -d core-api-stg.gdac.com.br
```

> O certbot atualiza o certificado existente e faz reload do nginx automaticamente.

### 4.6 Chave SSH para deploy automático

```bash
# No VPS (se ainda não criou)
ssh-keygen -t ed25519 -C "gdac-deploy" -f /root/.ssh/gdac_deploy -N ""
cat /root/.ssh/gdac_deploy.pub >> /root/.ssh/authorized_keys
chmod 600 /root/.ssh/authorized_keys

# Exibe a chave privada para copiar para o GitHub
cat /root/.ssh/gdac_deploy
```

---

## 5. VPS — arquivos .env

Os arquivos `.env` nos VPS são **gerados automaticamente pelos workflows de deploy** — não é necessário criá-los manualmente.

### O que o .env gerado contém

O workflow grava no VPS apenas os valores estáticos não-sensíveis:

```dotenv
REGISTRY=ghcr.io
IMAGE_NAME=<owner>/gdac-auth   # ou gdac-core
JWT_AUDIENCE=gdac-apps
```

### Como os secrets sensíveis chegam ao container

Senhas e chaves JWT **não são gravadas no `.env`**. Elas são injetadas diretamente no ambiente do container pelo `appleboy/ssh-action` via `envs:`, durante a execução do `docker compose up`. O container recebe as variáveis em tempo de execução sem que os valores toquem o disco do VPS em texto plano.

Secrets sensíveis gerenciados dessa forma:

- Senhas de banco: `AUTH_DB_PROD_PASSWORD`, `AUTH_DB_STG_PASSWORD`, `CORE_DB_PROD_PASSWORD`, `CORE_DB_STG_PASSWORD`
- Chaves JWT: `AUTH_JWT_PRIVATE_KEY`, `AUTH_JWT_PUBLIC_KEY`
- Senhas Redis: `AUTH_REDIS_PROD_PASSWORD`, `AUTH_REDIS_STG_PASSWORD`
- Senhas SMTP: `AUTH_EMAIL_PASSWORD`, `CORE_EMAIL_PASSWORD`

---

## 6. GitHub — secrets e permissões

### 6.1 Secrets necessários

Acesse **Settings → Secrets and variables → Actions** no repositório.

| Secret | Usado em | Descrição |
|--------|----------|-----------|
| `AUTH_DB_PROD_PASSWORD` | deploy-auth-production | Senha banco `gdac02` |
| `AUTH_DB_STG_PASSWORD` | deploy-auth-staging | Senha banco `gdac01` |
| `AUTH_JWT_PRIVATE_KEY` | ci-auth, deploy-auth-* | Chave privada RSA 4096 |
| `AUTH_JWT_PUBLIC_KEY` | ci-auth, ci-core, deploy-* | Chave pública RSA |
| `AUTH_REDIS_PROD_PASSWORD` | deploy-auth-production | Redis Auth prod |
| `AUTH_REDIS_STG_PASSWORD` | deploy-auth-staging | Redis Auth stg |
| `AUTH_EMAIL_PASSWORD` | deploy-auth-* | SMTP para Auth |
| `CORE_DB_PROD_PASSWORD` | deploy-core-production | Senha banco `gdac04` |
| `CORE_DB_STG_PASSWORD` | deploy-core-staging | Senha banco `gdac03` |
| `CORE_EMAIL_PASSWORD` | deploy-core-* | SMTP para Core |
| `PRODUCTION_HOST` / `PRODUCTION_USER` / `PRODUCTION_SSH_KEY` | deploy-*-production | Environment-level |
| `STAGING_HOST` / `STAGING_USER` / `STAGING_SSH_KEY` | deploy-*-staging | Environment-level |

### 6.2 Environments do GitHub

Crie dois environments em **Settings → Environments**:
- `production` — recomendado: adicionar revisores obrigatórios
- `staging`

### 6.3 Permissão para publicar no GHCR

Em **Settings → Actions → General** do repositório, habilite **Read and write permissions** para que o `GITHUB_TOKEN` possa publicar imagens no `ghcr.io`.

---

## 7. Branches e fluxo de deploy

```
develop ──────────────────────────────────────────────►
          │
          └──► PR → staging ──► CI ──► Deploy Auth Staging
          │                       └──► Deploy Core Staging
          │
          └──► PR → main    ──► CI ──► Deploy Auth Production
                                  └──► Deploy Core Production
```

| Branch | Trigger deploy | Auth | Core |
|--------|---------------|------|------|
| `develop` | — (nenhum) | – | – |
| `staging` | push → CI success | auth-stg.gdac.com.br | core-api-stg.gdac.com.br |
| `main` | push → CI success | auth.gdac.com.br | core-api.gdac.com.br |

**Deploy manual (re-deploy sem novo commit):**

```bash
gh workflow run deploy-auth-production.yml --field image_tag=latest
gh workflow run deploy-core-production.yml --field image_tag=latest
```

**Sincronizar staging e develop com main:**

```bash
git push origin main:staging main:develop
```

---

## 8. Primeiro deploy manual

Após configurar VPS e secrets:

```bash
# 1. Publicar imagens (disparar CI)
git push origin main

# 2. Aguardar CI (GitHub Actions)

# 3. Verificar saúde de todos os serviços
curl https://auth.gdac.com.br/health/ready
curl https://auth-stg.gdac.com.br/health/ready
curl https://core-api.gdac.com.br/health/ready
curl https://core-api-stg.gdac.com.br/health/ready
```

---

## 9. Banco de dados

MariaDB 11.4 externo em `mysql.gdac.com.br` (KingHost).

| Ambiente | Auth (banco) | Core (banco) | Servidor |
|----------|-------------|-------------|----------|
| Produção | `gdac02` | `gdac04` | `mysql.gdac.com.br` |
| Staging  | `gdac01` | `gdac03` | `mysql.gdac.com.br` |

### Tabelas por serviço

| Serviço | Tabelas |
|---------|---------|
| Auth | `users`, `refresh_tokens` |
| Core | `core_user_profiles`, `core_companies`, `core_company_members`, `core_company_offices`, `core_user_company_links` |

Migrations são aplicadas automaticamente na inicialização (`db.Database.Migrate()`).

### Backup antes do deploy (produção Auth)

O workflow `deploy-auth-production.yml` executa backup automático via `mariadb-dump` antes de subir a nova imagem. Para o Core, adicione o mesmo padrão conforme necessário.

---

## 10. E-mail (Serilog + SMTP KingHost)

| Configuração | Valor |
|-------------|-------|
| Servidor SMTP | `smtp.kinghost.net` |
| Porta | `465` |
| Criptografia | SSL/TLS (`SslOnConnect`) |
| Usuário | `admin.host@gdac.com.br` |
| Destinatário | `desenvolvimento@gdac.com.br` |

Subject automático por serviço e ambiente:

| Serviço | Produção | Outros ambientes |
|---------|----------|-----------------|
| Auth | `[URGENTE] [Production] [GDAC Auth] Erro crítico` | `[Staging] [GDAC Auth] Erro crítico` |
| Core | `[URGENTE] [Production] [GDAC Core] Erro crítico` | `[Staging] [GDAC Core] Erro crítico` |

A senha SMTP (`EMAIL_PASSWORD`) vem do `.env`. Para desenvolvimento local, use Mailpit (porta 1025, sem TLS).

---

## 11. Referência rápida de ambientes

### Gdac.Auth.Api

| | Local (dev) | Staging | Produção |
|---|---|---|---|
| URL | `http://localhost:5000` | `https://auth-stg.gdac.com.br` | `https://auth.gdac.com.br` |
| Branch | `develop` | `staging` | `main` |
| Porta interna | – | `8081` | `8080` |
| Banco | local (XAMPP) | `gdac01` | `gdac02` |
| Redis | `localhost:6379` | container `auth-redis` | container `auth-redis` |
| ASPNETCORE_ENVIRONMENT | `Development` | `Staging` | `Production` |
| Swagger | habilitado | habilitado | desabilitado |

### Gdac.Core.Api

| | Local (dev) | Staging | Produção |
|---|---|---|---|
| URL | `http://localhost:5269` | `https://core-api-stg.gdac.com.br` | `https://core-api.gdac.com.br` |
| Branch | `develop` | `staging` | `main` |
| Porta interna | – | `8083` | `8082` |
| Banco | local (XAMPP) | `gdac03` | `gdac04` |
| ASPNETCORE_ENVIRONMENT | `Development` | `Staging` | `Production` |
| Swagger | habilitado | habilitado | desabilitado |
| JWT | valida com chave pública do Auth stg | valida com chave pública do Auth stg | valida com chave pública do Auth prod |

### Comandos úteis no VPS

```bash
# Logs
docker logs gdac-auth-prod-auth-api-1 -f
docker logs gdac-auth-stg-auth-api-1 -f
docker logs gdac-core-prod-core-api-1 -f
docker logs gdac-core-stg-core-api-1 -f

# Status de todos os containers
docker ps --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'

# Reiniciar serviço de produção (Auth)
cd /opt/gdac/auth
IMAGE_TAG=latest docker compose -p gdac-auth-prod --env-file .env \
  -f docker/Auth/docker-compose.yml -f docker/Auth/docker-compose.prod.yml \
  restart auth-api

# Reiniciar serviço de produção (Core)
cd /opt/gdac/core
IMAGE_TAG=latest docker compose -p gdac-core-prod --env-file .env \
  -f docker/Core/docker-compose.yml -f docker/Core/docker-compose.prod.yml \
  restart core-api

# Renovação manual de certificado SSL
certbot renew --dry-run
```

### Portas por serviço

| Serviço | Dev | Staging | Produção |
|---------|-----|---------|---------|
| Auth | – | 8081 | 8080 |
| Core | – | 8083 | 8082 |
| Auth Redis | 6379 | interno | interno |

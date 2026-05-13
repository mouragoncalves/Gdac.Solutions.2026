# Guia de Infraestrutura e Deploy — GDAC Auth

Documenta toda a configuração necessária para reproduzir o ambiente de produção e staging do serviço `Gdac.Auth.Api` do zero.

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
            ├─> branch main    → Deploy Production (auth.gdac.com.br)
            └─> branch staging → Deploy Staging    (auth-stg.gdac.com.br)

VPS KingHost (Ubuntu 24.04)
    ├─ nginx (host) — SSL/TLS Let's Encrypt
    ├─ /opt/gdac/auth/       → prod  (porta interna 8080)
    └─ /opt/gdac/auth-stg/   → stg   (porta interna 8081)
```

**Stack:**
- Aplicação: .NET 10 / ASP.NET Core
- Banco: MariaDB 11.4 externo em `mysql.gdac.com.br`
- Cache: Redis 7.4-alpine (container Docker)
- Auth JWT: RS256, chaves RSA 4096 bits
- Container registry: GitHub Container Registry (`ghcr.io`)
- Proxy reverso: nginx no host
- SSL: Let's Encrypt (certbot), certificado SAN para ambos os domínios

---

## 2. Pré-requisitos

### Local
- Git, GitHub CLI (`gh`)
- OpenSSL
- Python 3 (para gerar o `.env` com chaves JWT formatadas)
- Acesso SSH ao VPS

### VPS
- Ubuntu 24.04 LTS
- Acesso root via SSH
- Domínios DNS apontando para o IP do VPS **antes** de rodar o certbot:

| Registro | Tipo | Valor |
|----------|------|-------|
| `auth.gdac.com.br` | A | `<IP do VPS>` |
| `auth-stg.gdac.com.br` | A | `<IP do VPS>` |

---

## 3. Chaves RSA JWT

As chaves são geradas **uma única vez** e reutilizadas em todos os ambientes (cada ambiente tem o seu par).

```bash
# Gerar par de chaves (4096 bits)
openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out jwt_private.pem
openssl rsa -pubout -in jwt_private.pem -out jwt_public.pem

# Visualizar em linha única (para colar no .env ou secrets)
awk 'NF {printf "%s\\n", $0}' jwt_private.pem
awk 'NF {printf "%s\\n", $0}' jwt_public.pem
```

> **Importante:** o `.env` no servidor e os secrets do GitHub devem conter as chaves com `\n` **literal** (não quebras de linha reais). O `RsaKeyProvider` converte `\n` → newline em tempo de execução.

Guarde os arquivos `.pem` fora do repositório (ex.: gerenciador de senhas ou vault).

---

## 4. VPS — provisionamento

### 4.1 Acessar o servidor

```bash
ssh -i ~/.ssh/gdac_vps_key root@<IP_DO_VPS>
```

### 4.2 Executar o script de setup

```bash
# No VPS, como root
curl -fsSL https://raw.githubusercontent.com/mouragoncalves/Gdac.Solutions.2026/main/Docs/Deploy/setup-vps.sh \
  | bash -s auth.gdac.com.br auth-stg.gdac.com.br suporte@gdac.com.br
```

O script realiza:
1. Atualiza pacotes do sistema
2. Instala Docker, nginx, certbot, ufw
3. Habilita firewall (SSH + HTTP/HTTPS)
4. Cria `/opt/gdac/auth` (prod) e `/opt/gdac/auth-stg` (staging)
5. Clona o repositório em ambos os diretórios
6. Configura e valida o nginx
7. Emite certificado Let's Encrypt (SAN: ambos os domínios)

### 4.3 Chave SSH para deploy automático

```bash
# No VPS
ssh-keygen -t ed25519 -C "gdac-deploy" -f /root/.ssh/gdac_deploy -N ""
cat /root/.ssh/gdac_deploy.pub >> /root/.ssh/authorized_keys
chmod 600 /root/.ssh/authorized_keys

# Exibe a chave privada para copiar para o GitHub
cat /root/.ssh/gdac_deploy
```

---

## 5. VPS — arquivos .env

Cada ambiente tem seu próprio `.env`. O Docker Compose lê o arquivo com `--env-file .env`.

### Formato

```dotenv
# /opt/gdac/auth/.env  (produção)
# /opt/gdac/auth-stg/.env  (staging)

DB_PROD_PASSWORD=senha_do_banco_prod
DB_STG_PASSWORD=senha_do_banco_stg
REDIS_PASSWORD=senha_redis_forte

# Chaves JWT em linha única com \n literal (não quebra de linha real)
JWT_PRIVATE_KEY="-----BEGIN PRIVATE KEY-----\nMIIEvgIBADA...\n-----END PRIVATE KEY-----"
JWT_PUBLIC_KEY="-----BEGIN PUBLIC KEY-----\nMIIBIjANBg...\n-----END PUBLIC KEY-----"

JWT_AUDIENCE=gdac-apps

EMAIL_PASSWORD=senha_smtp

REGISTRY=ghcr.io
IMAGE_NAME=mouragoncalves/gdac-auth
IMAGE_TAG=latest        # prod usa "latest"; staging usa o SHA do commit

ASPNETCORE_ENVIRONMENT=Production   # ou Staging
```

### Script Python para gerar o .env com chaves formatadas

Salve como `gerar_env.py` e execute **localmente** (nunca commite este arquivo):

```python
import json, pathlib, sys

secrets_file = pathlib.Path(sys.argv[1])  # JSON com as chaves
env_path     = pathlib.Path(sys.argv[2])  # caminho de saída

secrets = json.loads(secrets_file.read_text())

private = secrets["Jwt:PrivateKey"].replace("\n", "\\n")
public  = secrets["Jwt:PublicKey"].replace("\n", "\\n")

content = f"""DB_PROD_PASSWORD={secrets['db_password']}
REDIS_PASSWORD={secrets['redis_password']}
JWT_PRIVATE_KEY="{private}"
JWT_PUBLIC_KEY="{public}"
JWT_AUDIENCE=gdac-apps
EMAIL_PASSWORD={secrets['email_password']}
REGISTRY=ghcr.io
IMAGE_NAME=mouragoncalves/gdac-auth
IMAGE_TAG=latest
ASPNETCORE_ENVIRONMENT=Production
"""

env_path.write_text(content)
print(f"OK: {env_path}")
```

```bash
python3 gerar_env.py secrets_prod.json /tmp/env_prod
scp -i ~/.ssh/gdac_vps_key /tmp/env_prod root@<IP>:/opt/gdac/auth/.env
ssh -i ~/.ssh/gdac_vps_key root@<IP> "chmod 600 /opt/gdac/auth/.env"
rm /tmp/env_prod
```

---

## 6. GitHub — secrets e permissões

### 6.1 Secrets necessários

Acesse **Settings → Secrets and variables → Actions** no repositório.

| Secret | Onde usar | Valor |
|--------|-----------|-------|
| `PRODUCTION_HOST` | deploy-production.yml | IP do VPS |
| `PRODUCTION_USER` | deploy-production.yml | `root` |
| `PRODUCTION_SSH_KEY` | deploy-production.yml | Conteúdo de `/root/.ssh/gdac_deploy` |
| `DB_PROD_PASSWORD` | deploy-production.yml | Senha do banco `gdac02` |
| `STAGING_HOST` | deploy-staging.yml | IP do VPS (mesmo servidor) |
| `STAGING_USER` | deploy-staging.yml | `root` |
| `STAGING_SSH_KEY` | deploy-staging.yml | Mesma chave SSH (ou outra separada) |
| `DB_STG_PASSWORD` | deploy-staging.yml | Senha do banco `gdac01` |
| `CI_JWT_PRIVATE_KEY` | ci.yml (integration tests) | Chave privada JWT (linha única `\n`) |
| `CI_JWT_PUBLIC_KEY` | ci.yml (integration tests) | Chave pública JWT (linha única `\n`) |

### 6.2 Environments do GitHub

Crie dois environments em **Settings → Environments**:
- `production` — pode adicionar revisores obrigatórios
- `staging`

### 6.3 Permissão para publicar no GHCR

O CI usa `GITHUB_TOKEN` para publicar a imagem no `ghcr.io`. Certifique-se de que o repositório tem **Actions → Read and write permissions** habilitado em **Settings → Actions → General**.

---

## 7. Branches e fluxo de deploy

```
develop ──────────────────────────────────────────────►
                │
                └──► PR → staging ──► CI ──► Deploy Staging
                                │
                                └──► PR → main ──► CI ──► Deploy Production
```

| Branch | Ambiente | Deploy | Trigger |
|--------|----------|--------|---------|
| `develop` | (local/dev) | nenhum | – |
| `staging` | auth-stg.gdac.com.br | Automático | push → CI success |
| `main` | auth.gdac.com.br | Automático | push → CI success |

**Deploy manual de produção** (útil para re-deploy sem novo commit):

```bash
gh workflow run deploy-production.yml \
  --repo mouragoncalves/Gdac.Solutions.2026 \
  --field image_tag=latest
```

**Sincronizar staging/develop com main:**

```bash
git push origin main:staging main:develop
```

---

## 8. Primeiro deploy manual

Após configurar o VPS e os secrets, faça o primeiro deploy pela linha de comando:

```bash
# 1. Garantir que a imagem está publicada (rodar CI)
git push origin main

# 2. Aguardar CI concluir (acompanhe em https://github.com/mouragoncalves/Gdac.Solutions.2026/actions)

# 3. O deploy-production é disparado automaticamente após o CI.
#    Para forçar manualmente:
gh workflow run deploy-production.yml \
  --repo mouragoncalves/Gdac.Solutions.2026 \
  --field image_tag=latest

# 4. Verificar saúde
curl https://auth.gdac.com.br/health/ready
curl https://auth-stg.gdac.com.br/health/ready
```

---

## 9. Banco de dados

Banco MariaDB 11.4 externo em `mysql.gdac.com.br` (KingHost).

| Ambiente | Banco | Usuário |
|----------|-------|---------|
| Produção | `gdac02` | `gdac02` |
| Staging  | `gdac01` | `gdac01` |

### Migrations

As migrations são aplicadas automaticamente na inicialização da API via `db.Database.Migrate()` em `Program.cs`. Não há step de migration nos workflows de deploy.

### Backup automático (produção)

O workflow de deploy de produção executa um backup antes de atualizar a imagem:

```bash
docker run --rm mariadb:11.4 \
  mariadb-dump -h mysql.gdac.com.br -u gdac02 -p"${DB_PROD_PASSWORD}" gdac02 \
  > /opt/gdac/auth/backups/pre-deploy-$(date +%Y%m%d-%H%M%S).sql
```

> Para que o backup funcione, o usuário `gdac02` precisa de permissão `SELECT` concedida para o IP do VPS no painel KingHost → MySQL → Gerenciar usuários.

---

## 10. E-mail (Serilog + SMTP KingHost)

Alertas de erro (`LogEventLevel.Error+`) são enviados por e-mail via Serilog.

| Configuração | Valor |
|-------------|-------|
| Servidor SMTP | `smtp.kinghost.net` |
| Porta | `465` |
| Criptografia | SSL/TLS (`SslOnConnect`) |
| Usuário | `admin.host@gdac.com.br` |
| Destinatário | `desenvolvimento@gdac.com.br` |

O subject do e-mail identifica o ambiente automaticamente:
- Produção: `[URGENTE] [Production] [GDAC Auth] Erro crítico`
- Outros:   `[Staging] [GDAC Auth] Erro crítico`

A senha SMTP (`EMAIL_PASSWORD`) é lida do `.env` / variável de ambiente.

Para testar localmente, use o Mailpit (configurado em `appsettings.Development.json` com `SslMode: None`).

---

## 11. Referência rápida de ambientes

| | Local (dev) | Staging | Produção |
|---|---|---|---|
| URL | `http://localhost:5000` | `https://auth-stg.gdac.com.br` | `https://auth.gdac.com.br` |
| Branch | `develop` | `staging` | `main` |
| Porta interna | – | `8081` | `8080` |
| Banco | local / Docker | `gdac01` | `gdac02` |
| ASPNETCORE_ENVIRONMENT | `Development` | `Staging` | `Production` |
| Swagger | Habilitado | Habilitado | Desabilitado |
| E-mail | Mailpit | KingHost SMTP | KingHost SMTP |

### Comandos úteis no VPS

```bash
# Logs da API em produção
docker logs docker-auth-api-1 -f

# Logs da API em staging
docker logs docker-auth-stg-api-1 -f

# Reiniciar serviço de produção
cd /opt/gdac/auth
IMAGE_TAG=latest docker compose --env-file .env \
  -f docker/docker-compose.yml -f docker/docker-compose.prod.yml \
  restart auth-api

# Verificar containers rodando
docker ps --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'

# Renovação manual de certificado SSL
certbot renew --dry-run
```

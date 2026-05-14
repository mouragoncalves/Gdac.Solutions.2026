# Infraestrutura GDAC Solutions

Referência completa de secrets, bancos de dados, portas e padrões de deploy.

---

## Padrão de nomenclatura de secrets

Todo secret GitHub segue o padrão `<APP>_<RECURSO>_<VARIANTE>`:

- Prefixo da aplicação: `AUTH_`, `CORE_` (e futuramente `ACADEMY_`, etc.)
- Secrets de infraestrutura compartilhada (VPS/SSH) ficam sem prefixo de app pois são usados por todos

---

## Secrets por aplicação

### Auth (`Src/Auth`)

| Secret | Descrição | Obs |
|--------|-----------|-----|
| `AUTH_DB_PROD_PASSWORD` | Senha do banco gdac02 (produção) | |
| `AUTH_DB_STG_PASSWORD` | Senha do banco gdac01 (staging) | |
| `AUTH_JWT_PRIVATE_KEY` | Chave privada RS256 (assina tokens) | PEM multi-linha, base64 ou inline |
| `AUTH_JWT_PUBLIC_KEY` | Chave pública RS256 (valida tokens) | Compartilhada com Core |
| `AUTH_REDIS_PROD_PASSWORD` | Senha Redis produção | |
| `AUTH_REDIS_STG_PASSWORD` | Senha Redis staging | |
| `AUTH_EMAIL_PASSWORD` | Senha SMTP para envio de e-mails | |

### Core (`Src/Core`)

| Secret | Descrição | Obs |
|--------|-----------|-----|
| `CORE_DB_PROD_PASSWORD` | Senha do banco gdac04 (produção) | |
| `CORE_DB_STG_PASSWORD` | Senha do banco gdac03 (staging) | |
| `AUTH_JWT_PUBLIC_KEY` | Chave pública RS256 (valida tokens emitidos pelo Auth) | Mesma do Auth |
| `CORE_EMAIL_PASSWORD` | Senha SMTP para envio de e-mails | |

### CI (integração contínua — ambas as apps)

| Secret | Descrição |
|--------|-----------|
| `AUTH_JWT_PRIVATE_KEY` | Usado nos testes de integração do Auth |
| `AUTH_JWT_PUBLIC_KEY` | Usado nos testes de integração do Auth e do Core |

### Infraestrutura VPS (sem prefixo de app — environment-level)

Configurados nos environments `production` e `staging` do GitHub:

| Secret | Descrição |
|--------|-----------|
| `PRODUCTION_HOST` | IP ou hostname do VPS de produção |
| `PRODUCTION_USER` | Usuário SSH produção |
| `PRODUCTION_SSH_KEY` | Chave privada SSH produção |
| `STAGING_HOST` | IP ou hostname do VPS de staging |
| `STAGING_USER` | Usuário SSH staging |
| `STAGING_SSH_KEY` | Chave privada SSH staging |

---

## Bancos de dados

Servidor: `mysql.gdac.com.br` (alias DNS — não alterar; aponta para o host atual da Uni5)

> Padrão: nome do banco = nome do usuário MariaDB = `gdacNN`

| Banco | Usuário | Aplicação | Ambiente |
|-------|---------|-----------|----------|
| `gdac01` | `gdac01` | Auth | Staging |
| `gdac02` | `gdac02` | Auth | Production |
| `gdac03` | `gdac03` | Core | Staging |
| `gdac04` | `gdac04` | Core | Production |

---

## Portas (VPS)

Todos os containers escutam apenas em `127.0.0.1` (loopback). Nginx faz o proxy reverso para HTTPS.

| Porta | Serviço | Ambiente |
|-------|---------|----------|
| `8080` | Auth API prod | Production |
| `8081` | Auth API stg | Staging |
| `8082` | Core API prod | Production |
| `8083` | Core API stg | Staging |

---

## Estrutura de diretórios no VPS

```
/opt/gdac/
├── auth/          # Auth produção  (git clone do branch main)
├── auth-stg/      # Auth staging   (git clone do branch staging)
├── core/          # Core produção  (git clone do branch main)
└── core-stg/      # Core staging   (git clone do branch staging)
```

Cada diretório contém:
- Código-fonte (via `git pull` no deploy)
- `.env` — gerado automaticamente pelo workflow (apenas valores estáticos não-sensíveis)
- `backups/` — dumps SQL pré-deploy (apenas produção)

---

## Nomes dos projetos Docker Compose

| Projeto | Compose | Ambiente |
|---------|---------|----------|
| `gdac-auth-prod` | `docker/Auth/docker-compose.yml` + `.prod.yml` | Production |
| `gdac-auth-stg` | `docker/Auth/docker-compose.yml` + `.staging.yml` | Staging |
| `gdac-core-prod` | `docker/Core/docker-compose.yml` + `.prod.yml` | Production |
| `gdac-core-stg` | `docker/Core/docker-compose.yml` + `.staging.yml` | Staging |

Containers nomeados automaticamente: `<projeto>-<serviço>-1`  
Ex.: `gdac-auth-prod-auth-api-1`, `gdac-core-prod-core-api-1`

---

## Variáveis internas do Docker Compose

As variáveis abaixo são os nomes usados *dentro* dos arquivos `docker-compose.yml`. São alimentadas pelos secrets via `envs:` no workflow (variáveis de shell têm precedência sobre o `.env`).

### Auth

| Variável Compose | Secret GitHub | Fonte |
|-----------------|---------------|-------|
| `DB_PROD_PASSWORD` | `AUTH_DB_PROD_PASSWORD` | prod |
| `DB_STG_PASSWORD` | `AUTH_DB_STG_PASSWORD` | stg |
| `JWT_PRIVATE_KEY` | `AUTH_JWT_PRIVATE_KEY` | ambos |
| `JWT_PUBLIC_KEY` | `AUTH_JWT_PUBLIC_KEY` | ambos |
| `REDIS_PASSWORD` | `AUTH_REDIS_PROD_PASSWORD` / `AUTH_REDIS_STG_PASSWORD` | por env |
| `EMAIL_PASSWORD` | `AUTH_EMAIL_PASSWORD` | ambos |
| `REGISTRY` | — | gerado no `.env` |
| `IMAGE_NAME` | — | gerado no `.env` |
| `JWT_AUDIENCE` | — | gerado no `.env` (`gdac-apps`) |
| `IMAGE_TAG` | — | SHA do commit, injetado inline |

### Core

| Variável Compose | Secret GitHub | Fonte |
|-----------------|---------------|-------|
| `DB_PROD_PASSWORD` | `CORE_DB_PROD_PASSWORD` | prod |
| `DB_STG_PASSWORD` | `CORE_DB_STG_PASSWORD` | stg |
| `JWT_PUBLIC_KEY` | `AUTH_JWT_PUBLIC_KEY` | ambos |
| `EMAIL_PASSWORD` | `CORE_EMAIL_PASSWORD` | ambos |
| `REGISTRY` | — | gerado no `.env` |
| `IMAGE_NAME` | — | gerado no `.env` |
| `JWT_AUDIENCE` | — | gerado no `.env` (`gdac-apps`) |
| `IMAGE_TAG` | — | SHA do commit, injetado inline |

---

## Checklist — adicionar nova aplicação

1. **Banco de dados**: criar banco e usuário no servidor `mysql.gdac.com.br` (padrão: `gdacNN` = nome do banco = nome do usuário)
2. **Portas**: reservar 2 portas consecutivas no VPS (prod e stg)
3. **Diretórios VPS**: `/opt/gdac/<app>/` e `/opt/gdac/<app>-stg/`
4. **Secrets GitHub** (prefixo `<APP>_`):
   - `<APP>_DB_PROD_PASSWORD`
   - `<APP>_DB_STG_PASSWORD`
   - `<APP>_EMAIL_PASSWORD` (se a app envia e-mail)
   - Qualquer outro recurso próprio (Redis, S3, etc.)
5. **Workflows CI** (`ci-<app>.yml`): usar secrets `AUTH_JWT_PUBLIC_KEY` para validar JWT
6. **Workflows Deploy**: seguir padrão dos arquivos `deploy-core-*.yml`:
   - `workflow_run` + `workflow_dispatch`
   - Auto-provisioning com `git clone` na primeira execução
   - Gerar `.env` com valores estáticos; secrets via `envs:`
   - Health check com timeout e dump de logs em caso de falha (prod)
7. **Docker Compose**: usar nome de projeto `-p gdac-<app>-prod` / `-p gdac-<app>-stg`
8. **GHCR image**: `ghcr.io/${{ github.repository_owner }}/gdac-<app>`

---

## Imagens Docker (GHCR)

| Imagem | Tags |
|--------|------|
| `ghcr.io/mouragoncalves/gdac-auth` | `latest`, `staging`, `<SHA>` |
| `ghcr.io/mouragoncalves/gdac-core` | `latest`, `staging`, `<SHA>` |

Deploy sempre usa tag SHA para garantir idempotência.

---

## Comandos úteis

```bash
# Listar secrets do repositório
gh secret list

# Criar/atualizar secret
gh secret set AUTH_DB_PROD_PASSWORD --body "senha_aqui"

# Forçar deploy manual (workflow_dispatch)
gh workflow run "Deploy Auth Production" --field image_tag=<SHA>
gh workflow run "Deploy Core Production" --field image_tag=<SHA>

# Ver últimos runs de um workflow
gh run list --workflow=deploy-auth-production.yml

# Logs do último run
gh run view --log

# Logs do container no VPS
docker logs --tail 100 gdac-auth-prod-auth-api-1
docker logs --tail 100 gdac-core-prod-core-api-1
```

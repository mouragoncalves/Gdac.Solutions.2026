# Implantação GDAC — Checklist Mestre

**Este é o único documento a seguir para implantar ou dar manutenção em qualquer serviço da plataforma.**  
Nunca avance para a próxima fase sem concluir todas as caixas da fase atual.

---

## Status atual dos serviços

| Serviço | Dev local | CI | Staging | Produção |
|---------|-----------|-----|---------|---------|
| **Auth** | ✅ | ✅ | ✅ auth-stg.gdac.com.br | ✅ auth.gdac.com.br |
| **Core** | ✅ | ✅ | ❓ verificar | ✅ core-api.gdac.com.br |
| **Content** | ❓ appsettings vazio | ⚠️ sem testes | ❌ | ❌ |
| **Onboarding** | ✅ | ⚠️ fix em CI | ❌ | ❌ |

---

## Informações obrigatórias — solicitar ao usuário ANTES de implantar

Antes de iniciar qualquer fase de implantação de um serviço, obter do usuário:

| # | Informação | Para que serve | Obtido? |
|---|-----------|---------------|---------|
| 1 | **IP do servidor VPS de produção** | Registro DNS (A) | ❌ |
| 2 | **IP do servidor VPS de staging** | Registro DNS (A) | ❌ |
| 3 | **Senha do banco de staging** (`gdacXX`) | Secret `<APP>_DB_STG_PASSWORD` + docker-compose | ❌ |
| 4 | **Senha do banco de produção** (`gdacXX`) | Secret `<APP>_DB_PROD_PASSWORD` + docker-compose | ❌ |
| 5 | **Senha SMTP KingHost** | Secret `<APP>_EMAIL_PASSWORD` | ❌ |
| 6 | **Confirmação que registros DNS foram criados** | Pré-requisito do certbot | ❌ |

> **Regra:** nunca assumir que essas informações estão disponíveis. Perguntar explicitamente no início de cada serviço novo.

---

## Referência de portas e domínios

| Serviço | Prod | Staging | Domínio prod | Domínio staging |
|---------|------|---------|--------------|-----------------|
| Auth | 8080 | 8081 | auth.gdac.com.br | auth-stg.gdac.com.br |
| Core | 8082 | 8083 | core-api.gdac.com.br | core-api-stg.gdac.com.br |
| Content | 8084 | 8085 | content-api.gdac.com.br | content-api-stg.gdac.com.br |
| Onboarding | 8086 | 8087 | onboarding-api.gdac.com.br | onboarding-api-stg.gdac.com.br |
| Financial | 8088 | 8089 | financial-api.gdac.com.br | financial-api-stg.gdac.com.br |
| Billing | 8090 | 8091 | billing-api.gdac.com.br | billing-api-stg.gdac.com.br |
| Contract | 8092 | 8093 | contract-api.gdac.com.br | contract-api-stg.gdac.com.br |
| Notification | 8094 | 8095 | notification-api.gdac.com.br | notification-api-stg.gdac.com.br |

## Referência de bancos de dados

Servidor: `mysql.gdac.com.br` — padrão: nome do banco = nome do usuário = `gdacNN`

| Serviço | Staging | Produção |
|---------|---------|---------|
| Auth | gdac01 | gdac02 |
| Core | gdac03 | gdac04 |
| Content | gdac05 | gdac06 |
| Onboarding | gdac07 | gdac08 |
| Financial | gdac09 | gdac10 |
| Billing | gdac11 | gdac12 |
| Contract | gdac13 | gdac14 |
| Notification | gdac15 | gdac16 |

---

## Fase 0 — Infraestrutura base VPS (executar uma única vez)

Já executado para Auth e Core. Registrar aqui quando for replicado para outros VPS.

- [x] VPS Ubuntu 24.04 provisionado (KingHost)
- [x] Docker instalado
- [x] Nginx instalado no host
- [x] UFW configurado (portas 22, 80, 443)
- [x] Usuário de deploy criado e chave SSH configurada
- [x] Let's Encrypt / certbot instalado
- [x] Secrets de infraestrutura no GitHub:
  - [x] `PRODUCTION_HOST`, `PRODUCTION_USER`, `PRODUCTION_SSH_KEY`
  - [ ] `STAGING_HOST`, `STAGING_USER`, `STAGING_SSH_KEY`
- [x] Environments no GitHub criados: `production`, `staging`
- [x] GHCR: permissão "Read and write" habilitada em Settings → Actions → General

---

## Fase 1 — Desenvolvimento local (por serviço)

### 1.1 Código

- [ ] Projetos criados: `Domain`, `Application`, `Infrastructure`, `Api`
- [ ] Entidades, repositórios, handlers e controllers implementados
- [ ] `appsettings.json` — valores padrão (não vazio; nada sensível)
- [ ] `appsettings.Development.json` — conexão XAMPP local, Mailpit, Jwt issuer localhost
- [ ] `appsettings.Staging.json` — issuer staging, origens CORS staging
- [ ] `appsettings.Production.json` — issuer prod, origens CORS prod, Serilog JSON
- [ ] `Dockerfile` — padrão multi-stage, usuário `gdac` (UID 1001), porta 8080
- [ ] `dotnet build` — **0 erros e 0 warnings relevantes**

### 1.2 Banco de dados local

- [ ] Banco `gdac_<nome>_dev` criado no XAMPP local
- [ ] `dotnet ef migrations add InitialCreate --project Src/<Nome>/Gdac.<Nome>.Infrastructure --startup-project Src/<Nome>/Gdac.<Nome>.Api --output-dir Persistence/Migrations`
- [ ] Migration revisada e aplicada localmente

### 1.3 Testes

- [ ] Projeto `Gdac.<Nome>.UnitTests` criado e com testes cobrindo handlers, validators e domínio
- [ ] Projeto `Gdac.<Nome>.IntegrationTests` criado com `WebAppFactory` + Testcontainers
- [ ] `dotnet test` — **todos passando localmente**

---

## Fase 2 — Git e versionamento

- [ ] Código commitado na branch `develop`
- [ ] `git push origin develop`
- [ ] PR aberto de `develop` → `staging` após validação local
- [ ] PR aberto de `staging` → `main` após validação em staging

---

## Fase 3 — Docker Compose

Criar em `docker/<Nome>/`:

- [ ] `docker-compose.yml` — serviço base com variáveis de ambiente parametrizadas
- [ ] `docker-compose.dev.yml` — Mailpit + banco via host.docker.internal
- [ ] `docker-compose.staging.yml` — porta staging, `ASPNETCORE_ENVIRONMENT=Staging`
- [ ] `docker-compose.prod.yml` — porta prod, `ASPNETCORE_ENVIRONMENT=Production`

Padrão de porta nos compose: `127.0.0.1:<PORTA>:8080` (somente loopback, nginx faz o TLS)

---

## Fase 4 — Nginx

Criar em `docker/<Nome>/nginx/conf.d/`:

- [ ] `production.conf` — server_name, SSL Let's Encrypt, proxy_pass para porta interna prod
- [ ] `staging.conf` — server_name stg, SSL Let's Encrypt, proxy_pass para porta interna stg

Padrão: HTTP → redirect 301 HTTPS; TLS 1.2+1.3; HSTS; proxy com `X-Forwarded-*`.

---

## Fase 5 — GitHub Actions (CI)

Criar `.github/workflows/ci-<nome>.yml`:

- [ ] Trigger: `push` nas branches `main`, `staging`, `develop` + `pull_request`
- [ ] Job `build-and-test`:
  - Service MariaDB 11.4 com health check
  - Steps: checkout, setup .NET 10, restore, build, unit tests, integration tests, publish results
  - Env no step de integration tests: `ConnectionStrings__Default`, `Jwt__PublicKey: ${{ secrets.AUTH_JWT_PUBLIC_KEY }}`
- [ ] Job `docker-build` (após build-and-test): build e push para `ghcr.io/<owner>/gdac-<nome>:<sha>`
- [ ] `dotnet test` passando no CI antes de avançar

---

## Fase 6 — GitHub Actions (Deploy)

Criar dois workflows:

- [ ] `deploy-<nome>-staging.yml` — trigger: `workflow_run` (CI success, branch `staging`) + `workflow_dispatch`
- [ ] `deploy-<nome>-production.yml` — trigger: `workflow_run` (CI success, branch `main`) + `workflow_dispatch`

Padrão de cada workflow:
1. Resolve SHA da imagem
2. SSH no VPS via `appleboy/ssh-action`
3. `git clone` na primeira execução ou `git pull`
4. Grava `.env` com valores não-sensíveis: `REGISTRY`, `IMAGE_NAME`, `JWT_AUDIENCE`
5. `docker pull <imagem>:<sha>`
6. `docker compose -p gdac-<nome>-prod ... up -d --force-recreate`
7. Health check com `timeout 90` — se falhar, imprime logs e retorna erro

---

## Fase 7 — GitHub Secrets

Criar em **Settings → Secrets and variables → Actions**:

- [ ] `<NOME>_DB_STG_PASSWORD` — senha banco `gdacXX` staging ← **solicitar ao usuário**
- [ ] `<NOME>_DB_PROD_PASSWORD` — senha banco `gdacXX` produção ← **solicitar ao usuário**
- [ ] `<NOME>_EMAIL_PASSWORD` — senha SMTP KingHost ← **solicitar ao usuário**

Secrets reutilizados (já existem):
- `AUTH_JWT_PUBLIC_KEY` — chave pública RSA para validar tokens
- `PRODUCTION_HOST`, `PRODUCTION_USER`, `PRODUCTION_SSH_KEY`
- `STAGING_HOST`, `STAGING_USER`, `STAGING_SSH_KEY`

---

## Fase 8 — DNS

**Solicitar ao usuário o IP do VPS antes deste passo.**

Criar no painel DNS do provedor (KingHost):

| Tipo | Nome | Valor | TTL |
|------|------|-------|-----|
| A | `<nome>-api` | `<IP do VPS prod>` | 3600 |
| A | `<nome>-api-stg` | `<IP do VPS staging>` | 3600 |

> Aguardar propagação DNS (até 24h; testar com `dig <domínio> +short`).  
> O certbot falha se o DNS não estiver propagado — **não avançar sem confirmar**.

---

## Fase 9 — VPS — Configuração

Executar no VPS via SSH:

```bash
# Criar diretórios
mkdir -p /opt/gdac/<nome> /opt/gdac/<nome>-stg

# Clonar repositório
git clone https://github.com/<owner>/Gdac.Solutions.2026.git /opt/gdac/<nome>
git clone https://github.com/<owner>/Gdac.Solutions.2026.git /opt/gdac/<nome>-stg
cd /opt/gdac/<nome>-stg && git checkout staging

# Nginx — copiar configurações
cp /opt/gdac/<nome>/docker/<Nome>/nginx/conf.d/production.conf /etc/nginx/conf.d/<nome>-production.conf
cp /opt/gdac/<nome>/docker/<Nome>/nginx/conf.d/staging.conf    /etc/nginx/conf.d/<nome>-staging.conf

# Validar nginx (sem SSL ainda — comentar blocos SSL temporariamente se necessário)
nginx -t && systemctl reload nginx
```

Checklist:
- [ ] Diretórios criados no VPS
- [ ] Repositório clonado em prod e stg
- [ ] Configs nginx copiadas e ativas

---

## Fase 10 — Certificado SSL (Let's Encrypt)

Expande o certificado SAN existente adicionando os novos domínios:

```bash
certbot --nginx \
  -d auth.gdac.com.br \
  -d auth-stg.gdac.com.br \
  -d core-api.gdac.com.br \
  -d core-api-stg.gdac.com.br \
  -d content-api.gdac.com.br \
  -d content-api-stg.gdac.com.br \
  -d onboarding-api.gdac.com.br \
  -d onboarding-api-stg.gdac.com.br
```

> Adicionar todos os domínios de uma vez quando possível.  
> Pré-requisito: **DNS propagado** para todos os domínios novos.

- [ ] Certbot executado com sucesso
- [ ] `nginx -t && systemctl reload nginx` após certbot
- [ ] HTTPS acessível em todos os domínios: `curl -I https://<domínio>/health/live`

---

## Fase 11 — Primeiro deploy

```bash
# Disparar CI (commit no main ou staging)
git push origin main      # dispara CI Core/Content/Onboarding prod
git push origin staging   # dispara CI staging
```

Ou manualmente (re-deploy sem novo commit):

```bash
gh workflow run deploy-<nome>-production.yml --field image_tag=latest
gh workflow run deploy-<nome>-staging.yml --field image_tag=latest
```

- [ ] CI passou (build + testes + docker push)
- [ ] Deploy workflow executou sem erro
- [ ] Health checks respondendo:

```bash
curl https://<nome>-api.gdac.com.br/health/ready
curl https://<nome>-api-stg.gdac.com.br/health/ready
# Esperado: {"status":"Healthy","checks":{"database":"Healthy"}}
```

---

## Fase 12 — Validação pós-deploy

- [ ] `GET /health/live` retorna 200 (liveness)
- [ ] `GET /health/ready` retorna `{"status":"Healthy"}` (readiness + banco)
- [ ] Swagger acessível em staging: `https://<nome>-api-stg.gdac.com.br/swagger`
- [ ] Swagger **não** acessível em produção (retorna 404)
- [ ] Log de container limpo (sem exceções na inicialização): `docker logs --tail 50 gdac-<nome>-prod-<nome>-api-1`
- [ ] Teste de autenticação: requisição com JWT inválido retorna 401

---

## Pendências imediatas — Content e Onboarding

### O que ainda falta para colocar os dois no ar

| Item | Content | Onboarding |
|------|---------|------------|
| appsettings.json correto | ❌ (template vazio) | ✅ |
| appsettings por ambiente | ❌ | ✅ |
| CI passando | ⚠️ sem testes | ⚠️ fix em andamento |
| GitHub secrets | ❌ | ❌ |
| DNS | ❌ | ❌ |
| VPS diretórios + nginx | ❌ | ❌ |
| Certbot expandido | ❌ | ❌ |

### Próximos passos em ordem

1. **Usuário fornece:** IP do VPS, senhas dos bancos gdac05/06 e gdac07/08, senha SMTP
2. Corrigir `appsettings.json` do Content (está com template vazio)
3. Criar registros DNS para os 4 domínios (content + onboarding, prod + stg)
4. Criar os 4 GitHub secrets em falta
5. VPS: mkdir, git clone, copiar nginx configs
6. Certbot: expandir certificado com os 4 novos domínios
7. Push → CI → deploy automático

---

## Comandos de diagnóstico rápido

```bash
# Ver todos os containers em execução no VPS
docker ps --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'

# Logs de um serviço
docker logs --tail 100 -f gdac-<nome>-prod-<nome>-api-1

# Forçar re-deploy manual
gh workflow run deploy-<nome>-production.yml --field image_tag=latest

# Verificar propagação DNS
dig content-api.gdac.com.br +short
dig onboarding-api.gdac.com.br +short

# Listar secrets configurados no GitHub
gh secret list

# Renovação manual SSL (+ dry-run antes)
certbot renew --dry-run
certbot renew

# Sincronizar branches
git push origin main:staging main:develop
```

---

## Referência de secrets completa

| Secret | Usado por | Descrição |
|--------|-----------|-----------|
| `AUTH_JWT_PRIVATE_KEY` | ci-auth, deploy-auth-* | Chave privada RS256 — somente Auth emite tokens |
| `AUTH_JWT_PUBLIC_KEY` | ci-*, deploy-* (todos) | Chave pública RSA — compartilhada com todos os serviços |
| `AUTH_DB_PROD_PASSWORD` | deploy-auth-production | Banco gdac02 |
| `AUTH_DB_STG_PASSWORD` | deploy-auth-staging | Banco gdac01 |
| `AUTH_REDIS_PROD_PASSWORD` | deploy-auth-production | Redis Auth prod |
| `AUTH_REDIS_STG_PASSWORD` | deploy-auth-staging | Redis Auth stg |
| `AUTH_EMAIL_PASSWORD` | deploy-auth-* | SMTP KingHost |
| `CORE_DB_PROD_PASSWORD` | deploy-core-production | Banco gdac04 |
| `CORE_DB_STG_PASSWORD` | deploy-core-staging | Banco gdac03 |
| `CORE_EMAIL_PASSWORD` | deploy-core-* | SMTP KingHost |
| `CONTENT_DB_PROD_PASSWORD` | deploy-content-production | Banco gdac06 |
| `CONTENT_DB_STG_PASSWORD` | deploy-content-staging | Banco gdac05 |
| `CONTENT_EMAIL_PASSWORD` | deploy-content-* | SMTP KingHost |
| `ONBOARDING_DB_PROD_PASSWORD` | deploy-onboarding-production | Banco gdac08 |
| `ONBOARDING_DB_STG_PASSWORD` | deploy-onboarding-staging | Banco gdac07 |
| `ONBOARDING_EMAIL_PASSWORD` | deploy-onboarding-* | SMTP KingHost |
| `PRODUCTION_HOST` | deploy-*-production | IP/hostname VPS prod |
| `PRODUCTION_USER` | deploy-*-production | Usuário SSH prod |
| `PRODUCTION_SSH_KEY` | deploy-*-production | Chave privada SSH prod |
| `STAGING_HOST` | deploy-*-staging | IP/hostname VPS staging |
| `STAGING_USER` | deploy-*-staging | Usuário SSH staging |
| `STAGING_SSH_KEY` | deploy-*-staging | Chave privada SSH staging |

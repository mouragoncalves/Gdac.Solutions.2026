# Checklist — Novo Serviço Backend

Ordem obrigatória para adicionar um novo serviço `.NET` à plataforma GDAC.  
Siga cada passo antes de avançar para o próximo.

---

## Fase 1 — Planejamento

- [ ] Definir nome do serviço (`Gdac.<Nome>.Api`)
- [ ] Definir entidades de domínio e responsabilidades
- [ ] Definir porta interna (prod e staging)
- [ ] Definir domínio público (`<nome>-api.gdac.com.br` e `<nome>-api-stg.gdac.com.br`)
- [ ] **Solicitar ao usuário:** nome e senha dos bancos de staging e produção

---

## Fase 2 — Código

- [ ] Criar os 4 projetos: `Domain`, `Application`, `Infrastructure`, `Api`
- [ ] Implementar entidades, repositórios, handlers e controllers
- [ ] Configurar `appsettings.json` / `Development` / `Staging` / `Production`
- [ ] Criar `Dockerfile` (padrão multi-stage)
- [ ] Executar `dotnet build` — **0 erros antes de continuar**

---

## Fase 3 — GitHub Secrets

Criar no repositório em **Settings → Secrets and variables → Actions**:

- [ ] `<NOME>_DB_STG_PASSWORD`
- [ ] `<NOME>_DB_PROD_PASSWORD`
- [ ] `<NOME>_EMAIL_PASSWORD`

> Secrets de infraestrutura (`PRODUCTION_HOST`, `STAGING_HOST`, `*_SSH_KEY`, `AUTH_JWT_PUBLIC_KEY`) já existem e são reutilizados por todos os serviços.

---

## Fase 4 — Migration

- [ ] Confirmar que o banco de dev existe (XAMPP local, `gdac_<nome>_dev`)
- [ ] Executar:
  ```bash
  dotnet ef migrations add InitialCreate \
    --project Src/<Nome>/Gdac.<Nome>.Infrastructure \
    --startup-project Src/<Nome>/Gdac.<Nome>.Api \
    --output-dir Persistence/Migrations
  ```
- [ ] Revisar o arquivo de migration gerado

---

## Fase 5 — Docker Compose

Criar em `docker/<Nome>/`:

- [ ] `docker-compose.yml` — serviços base (api)
- [ ] `docker-compose.staging.yml` — porta staging, env staging
- [ ] `docker-compose.prod.yml` — porta prod, env prod

---

## Fase 6 — Nginx

- [ ] Criar `docker/<Nome>/nginx/conf.d/production.conf`
- [ ] Criar `docker/<Nome>/nginx/conf.d/staging.conf`

---

## Fase 7 — Workflows CI/CD

Criar em `.github/workflows/`:

- [ ] `ci-<nome>.yml` — build, test, docker push → ghcr.io
- [ ] `deploy-<nome>-staging.yml` — triggered por CI success em `staging`
- [ ] `deploy-<nome>-production.yml` — triggered por CI success em `main` + `workflow_dispatch`

---

## Fase 8 — VPS

Executar no VPS (`ssh root@<IP>`):

- [ ] `mkdir -p /opt/gdac/<nome> /opt/gdac/<nome>-stg`
- [ ] Clonar repositório nos dois diretórios
- [ ] Copiar configs nginx: `cp .../production.conf /etc/nginx/conf.d/<nome>-production.conf`
- [ ] Copiar configs nginx staging
- [ ] Expandir certificado SSL com certbot (adicionar novos domínios ao SAN existente)
- [ ] `nginx -t && systemctl reload nginx`

---

## Fase 9 — Primeiro Deploy

- [ ] `git push origin main` para disparar CI
- [ ] Aguardar CI concluir (GitHub Actions)
- [ ] Verificar health checks:
  ```bash
  curl https://<nome>-api.gdac.com.br/health/ready
  curl https://<nome>-api-stg.gdac.com.br/health/ready
  ```

---

## Fase 10 — Documentação

- [ ] Criar `Docs/<Nome>/README.md`
- [ ] Criar `Docs/<Nome>/01-arquitetura.md`
- [ ] Criar `Docs/<Nome>/02-entidades.md`
- [ ] Criar `Docs/<Nome>/03-endpoints.md`
- [ ] Criar `Docs/<Nome>/04-infraestrutura.md`
- [ ] Atualizar `Docs/Deploy/DEPLOY-SETUP.md` com novo serviço

---

## Referência de portas

| Serviço | Prod | Staging |
|---------|------|---------|
| Auth | 8080 | 8081 |
| Core | 8082 | 8083 |
| Content | 8084 | 8085 |
| Onboarding | 8086 | 8087 |
| Financial | 8088 | 8089 |
| Billing | 8090 | 8091 |
| Contract | 8092 | 8093 |
| Notification | 8094 | 8095 |

## Referência de bancos

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

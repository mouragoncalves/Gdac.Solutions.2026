# Infraestrutura — Gdac.Auth

## Ambientes

### Development (local)

| Característica | Valor |
|----------------|-------|
| Swagger | Habilitado (CSP relaxada para `/swagger/*`) |
| Logs | Nível Debug, console colorido |
| Migrations | Manuais via `dotnet ef database update` |
| HTTPS | Certificado de desenvolvimento .NET |
| Redis | Container Docker (`auth-redis-dev` na porta 6379) |
| MariaDB | XAMPP local via socket Unix (`/Applications/XAMPP/xamppfiles/var/mysql/mysql.sock`) |
| E-mail | Mailpit container Docker (porta 1025 SMTP / 8025 UI) |

**Pré-requisitos para rodar localmente:**
```bash
# 1. Iniciar XAMPP (MariaDB 10.4+)
# 2. Redis
docker run -d --name auth-redis-dev -p 6379:6379 redis:7.4-alpine redis-server --save ""
# 3. Mailpit
docker run -d --name mailpit-dev -p 8025:8025 -p 1025:1025 axllent/mailpit:latest
# 4. Criar banco
/Applications/XAMPP/xamppfiles/bin/mysql --socket=... -u root -e "CREATE DATABASE gdac_auth_dev ..."
# 5. Aplicar migrations
dotnet ef database update --project Src/Gdac.Auth.Infrastructure --startup-project Src/Gdac.Auth.Api
# 6. Configurar user-secrets (chaves JWT)
cd Src/Gdac.Auth.Api && dotnet user-secrets set "Jwt:PrivateKey" "$(cat ../../secrets/jwt_private.pem)"
# 7. Rodar a API
dotnet run --project Src/Gdac.Auth.Api
```

Arquivo: `appsettings.Development.json`

---

### Staging (homologação)

| Característica | Valor |
|----------------|-------|
| Swagger | Habilitado (CSP relaxada para `/swagger/*`) |
| Logs | Nível Information, JSON estruturado |
| Migrations | Executadas no deploy via `docker run` antes do `compose up` |
| HTTPS | Certificado real (Let's Encrypt via Nginx) |
| Redis | Container dedicado |
| MariaDB | `mysql.gdac.com.br` — database `gdac_auth_stg`, usuário `gdac01` |

Arquivo: `appsettings.Staging.json`

---

### Production (produção)

| Característica | Valor |
|----------------|-------|
| Swagger | Desabilitado |
| Logs | Nível Warning + eventos de segurança |
| Migrations | Executadas no deploy — backup automático antes via `mysqldump` |
| HTTPS | Obrigatório, HSTS ativo |
| Rate Limit | Totalmente ativo |
| Debug | Desabilitado |
| MariaDB | `mysql.gdac.com.br` — database `gdac_auth`, usuário `gdac02` |

Arquivo: `appsettings.Production.json`

---

## Docker

### Estrutura

```
Src/Gdac.Auth.Api/
├── Dockerfile
└── .dockerignore

docker/
├── docker-compose.yml             ← base (serviços compartilhados)
├── docker-compose.dev.yml         ← override development
├── docker-compose.staging.yml     ← override staging
└── docker-compose.prod.yml        ← override produção
```

### Dockerfile (multi-stage)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Src/Gdac.Auth.Api/Gdac.Auth.Api.csproj
RUN dotnet publish Src/Gdac.Auth.Api/Gdac.Auth.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Gdac.Auth.Api.dll"]
```

### docker-compose.yml (base)

Serviços:
- `auth-api` — a aplicação
- `auth-redis` — Redis
- `auth-nginx` — Nginx reverse proxy (staging/prod)

> O banco de dados **não é gerenciado via container** — dev usa XAMPP local, staging e prod usam `mysql.gdac.com.br`.

---

## Nginx

Configuração base para reverse proxy com HTTPS:

```nginx
server {
    listen 443 ssl http2;
    server_name auth.gdac.com.br;

    ssl_certificate /etc/nginx/certs/fullchain.pem;
    ssl_certificate_key /etc/nginx/certs/privkey.pem;

    location / {
        proxy_pass http://gdac-auth-api:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

server {
    listen 80;
    server_name auth.gdac.com.br;
    return 301 https://$host$request_uri;
}
```

---

## Health Checks

Dois endpoints distintos:

### Liveness — `GET /health/live`

Verifica se a aplicação está rodando (sem dependências externas).

```json
{ "status": "Healthy" }
```

### Readiness — `GET /health/ready`

Verifica se todas as dependências estão disponíveis:

```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "redis": "Healthy"
  }
}
```

| Status | Cenário |
|--------|---------|
| 200 | Tudo OK |
| 503 | Alguma dependência falhando |

Usados pelo Docker/orquestrador para decidir se direciona tráfego ao container.

---

## Observabilidade

### Serilog

Sinks por ambiente:

| Ambiente | Sinks |
|----------|-------|
| Development | Console (colorido e legível) |
| Staging | Console (JSON) + arquivo rotativo |
| Production | Console (JSON) — integração com stack de logs centralizado |

### Campos obrigatórios em todo log

```json
{
  "Timestamp": "2026-05-13T10:00:00Z",
  "Level": "Information",
  "Message": "Login realizado com sucesso",
  "CorrelationId": "abc123",
  "UserId": "guid",
  "ApplicationId": "guid",
  "IpAddress": "192.168.1.1",
  "RequestPath": "/api/v1/auth/login"
}
```

---

## CI/CD — GitHub Actions

### Pipelines

#### `ci.yml` — roda em todo PR e push para main

```
Etapas:
1. Checkout
2. Setup .NET 10
3. Restore dependencies
4. Build
5. Run unit tests (Gdac.Auth.UnitTests)
6. Run integration tests (Gdac.Auth.IntegrationTests) ← usa MariaDB e Redis via service containers
7. Publish coverage report
```

#### `deploy-staging.yml` — roda automaticamente após merge em main

```
Etapas:
1. Build da imagem Docker
2. Push para registry (GHCR)
3. SSH no servidor de staging
4. docker pull + docker-compose up -d
5. Health check — aguarda /health/ready retornar 200
6. Notificação de resultado
```

#### `deploy-production.yml` — deploy manual (workflow_dispatch)

```
Etapas:
1. Requer confirmação manual no GitHub
2. Backup automático do banco (dump via mysqldump)
3. Build da imagem Docker (tag de release)
4. Push para registry
5. SSH no servidor de produção
6. docker pull + docker-compose up -d
7. Health check
8. Notificação de resultado
```

### Secrets necessários no GitHub

**Repositório** (Settings → Secrets → Actions):

```
CI_JWT_PRIVATE_KEY      ← chave RSA privada (gerada por scripts/generate-keys.sh)
CI_JWT_PUBLIC_KEY       ← chave RSA pública correspondente
```

**Ambiente `production`** (Settings → Environments → production):

```
DB_PROD_PASSWORD        ← senha do usuário gdac02 em mysql.gdac.com.br
PRODUCTION_HOST         ← mysql.gdac.com.br
PRODUCTION_USER         ← usuário SSH do servidor
PRODUCTION_SSH_KEY      ← chave privada SSH
```

**Ambiente `staging`** (Settings → Environments → staging):

```
DB_STG_PASSWORD         ← senha do usuário gdac01 em mysql.gdac.com.br
STAGING_HOST            ← mysql.gdac.com.br
STAGING_USER            ← usuário SSH do servidor
STAGING_SSH_KEY         ← chave privada SSH
```

---

## Geração do Par de Chaves RSA

O par de chaves RSA para assinatura JWT deve ser gerado uma única vez por ambiente e armazenado com segurança:

```bash
# Gerar chave privada (2048 bits mínimo, 4096 recomendado)
openssl genrsa -out private.pem 4096

# Extrair chave pública
openssl rsa -in private.pem -pubout -out public.pem
```

- Em development: configurar via `dotnet user-secrets` (ver pré-requisitos acima)
- Em staging/production: variáveis de ambiente injetadas pelo docker compose via `.env` no servidor (nunca em arquivo commitado)

O script de geração de chaves está em `scripts/generate-keys.sh` e salva os arquivos em `secrets/` (pasta no `.gitignore`).

Rotação de chaves: anualmente ou imediatamente em caso de comprometimento. Ao rotacionar, manter a chave antiga no JWKS até todos os tokens com ela expirarem (máximo 15 minutos + margem).

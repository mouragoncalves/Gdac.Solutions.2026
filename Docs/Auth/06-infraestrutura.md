# Infraestrutura — Gdac.Auth

## Ambientes

### Development (local)

| Característica | Valor |
|----------------|-------|
| Swagger | Habilitado e público |
| Logs | Nível Debug, console colorido |
| Migrations | Automáticas ao iniciar |
| HTTPS | Certificado de desenvolvimento .NET |
| Redis | Container local |
| MariaDB | Container local |

Arquivo: `appsettings.Development.json`

---

### Staging (homologação)

| Característica | Valor |
|----------------|-------|
| Swagger | Habilitado, protegido por Basic Auth |
| Logs | Nível Information, JSON estruturado |
| Migrations | Manuais (via CLI antes do deploy) |
| HTTPS | Certificado real (Let's Encrypt via Nginx) |
| Redis | Instância dedicada |
| MariaDB | Banco separado do produção |

Arquivo: `appsettings.Staging.json`

---

### Production (produção)

| Característica | Valor |
|----------------|-------|
| Swagger | Desabilitado |
| Logs | Nível Warning + eventos de segurança |
| Migrations | Manuais, com backup obrigatório antes |
| HTTPS | Obrigatório, HSTS ativo |
| Rate Limit | Totalmente ativo |
| Debug | Desabilitado |

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
- `gdac-auth-api` — a aplicação
- `gdac-auth-db` — MariaDB
- `gdac-auth-redis` — Redis
- `gdac-auth-nginx` — Nginx reverse proxy

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

```
REGISTRY_TOKEN          ← token GHCR
STAGING_SSH_KEY         ← chave SSH staging
STAGING_HOST            ← IP/hostname staging
PRODUCTION_SSH_KEY      ← chave SSH produção
PRODUCTION_HOST         ← IP/hostname produção
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

- Em development: configurar via `dotnet user-secrets`
- Em staging/production: variáveis de ambiente no servidor (nunca em arquivo commitado)

Rotação de chaves: anualmente ou imediatamente em caso de comprometimento. Ao rotacionar, manter a chave antiga no JWKS até todos os tokens com ela expirarem (máximo 15 minutos + margem).

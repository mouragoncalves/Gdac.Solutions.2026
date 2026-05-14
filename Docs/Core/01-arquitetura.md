# Arquitetura — Gdac.Core.Api

## Visão geral

O `Gdac.Core.Api` é um serviço de suporte que serve dados compartilhados para toda a plataforma GDAC:

- **Perfis de usuário**: complementam os dados de autenticação do Auth (nome, CPF, foto, etc.)
- **Empresas**: cadastro de clientes, parceiros e empresas internas
- **Vínculo usuário-empresa**: papel (role) de cada usuário em cada empresa

Qualquer outra API da plataforma pode consultar o Core para obter esses dados sem precisar recadastrar usuários.

## Estrutura de camadas

```
Gdac.Core.Api              ← ASP.NET Core, Controllers, Middleware, DI
Gdac.Core.Application      ← CQRS com MediatR, Validators, Use Cases
Gdac.Core.Infrastructure   ← EF Core, Repositories, EmailService
Gdac.Core.Domain           ← Entidades, Interfaces, Exceções, Enums
```

### Regra de dependência (Clean Architecture)

```
Api → Application → Domain
Infrastructure → Domain
Api → Infrastructure (somente via DI em Program.cs)
```

A camada de domínio não depende de nada. Todas as demais dependem do domínio. A Application nunca referencia diretamente Infrastructure ou Api.

## Fluxo de uma requisição

```
HTTP Request
    └─> CorrelationIdMiddleware (injeta X-Correlation-Id)
    └─> ExceptionHandlingMiddleware (captura exceções → HTTP status)
    └─> SecurityHeadersMiddleware (headers de segurança)
    └─> [Autenticação JWT] (valida Bearer token)
    └─> Controller
            └─> mediator.Send(Command/Query)
                    └─> ValidationBehavior (FluentValidation)
                    └─> LoggingBehavior (Serilog)
                    └─> Handler
                            └─> Repository → MariaDB
                    └─> Response DTO
```

## Projetos de teste

```
Tests/Core/Gdac.Core.UnitTests        ← handlers, validators (sem I/O)
Tests/Core/Gdac.Core.IntegrationTests ← endpoints HTTP com Testcontainers.MySql
```

## Comunicação com Auth

O Core **não chama** o Auth diretamente. A relação é:
- Auth emite tokens JWT RS256
- Core recebe o token JWT no header `Authorization: Bearer <token>` e valida usando a **chave pública** do Auth
- O `sub` claim do token é o `UserId` (GUID), que o Core usa para associar perfis

A chave pública é configurada via `Jwt:PublicKey` no `.env` / appsettings.

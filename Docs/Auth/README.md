# Gdac.Auth — Documentação Técnica

API central de autenticação enterprise para todas as aplicações GDAC.

---

## Índice

| # | Documento | Conteúdo |
|---|-----------|----------|
| 1 | [Arquitetura](./01-arquitetura.md) | Decisões técnicas, estrutura da solution, camadas |
| 2 | [Entidades](./02-entidades.md) | Modelo de dados, relacionamentos, índices |
| 3 | [Autenticação](./03-autenticacao.md) | JWT, tokens, sessões, fluxos detalhados |
| 4 | [Endpoints](./04-endpoints.md) | Referência completa da API |
| 5 | [Segurança](./05-seguranca.md) | Senhas, rate limit, headers, CORS, brute force |
| 6 | [Infraestrutura](./06-infraestrutura.md) | Docker, ambientes, CI/CD, observabilidade |
| 7 | [Swagger](./07-swagger.md) | Como testar a API localmente via Swagger |

---

## Visão Geral

A Gdac.Auth é responsável exclusivamente por:

- Autenticação de usuários e aplicações
- Emissão e renovação de tokens JWT (RS256)
- Gerenciamento de sessões
- Controle de quais aplicações e empresas cada usuário acessa

A API **não** contém regras de negócio, não acessa bancos das aplicações e não armazena dados completos de usuários ou empresas.

---

## Stack

| Componente | Tecnologia |
|------------|-----------|
| Runtime | .NET 10 |
| Framework | ASP.NET Core |
| ORM | Entity Framework Core |
| Banco | MariaDB/MySQL |
| Cache | Redis |
| Tokens | JWT customizado (RS256) |
| Senhas | Argon2id (fallback BCrypt) |
| Validação | FluentValidation |
| Mediator | MediatR (CQRS) |
| Logs | Serilog |
| Proxy | Nginx |
| Containers | Docker + Docker Compose |
| CI/CD | GitHub Actions |

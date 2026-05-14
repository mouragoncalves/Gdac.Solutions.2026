# Gdac.Core.Api — Documentação

Serviço centralizado de perfis de usuário e diretório de empresas da plataforma GDAC.

## Índice

1. [01-arquitetura.md](01-arquitetura.md) — Visão geral, camadas, fluxo de requisição
2. [02-entidades.md](02-entidades.md) — Entidades de domínio e regras de negócio
3. [03-autorizacao.md](03-autorizacao.md) — JWT RS256, validação de tokens do Auth
4. [04-endpoints.md](04-endpoints.md) — Todos os endpoints REST disponíveis
5. [05-infraestrutura.md](05-infraestrutura.md) — Banco de dados, migrações, serviços

## Resumo

| | |
|---|---|
| Porta (dev) | 5269 |
| Porta (staging) | 8083 |
| Porta (produção) | 8082 |
| Banco (prod) | MariaDB `gdac02` (tabelas `core_*`) |
| JWT | Valida tokens emitidos pelo `Gdac.Auth.Api` |
| Redis | Não usa |

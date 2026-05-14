# GDAC Platform — Visão Geral

Documento de referência para planejamento da plataforma completa GDAC.  
Captura decisões, arquitetura e pontos em aberto para refinamento.

---

## 1. O que é a plataforma

A GDAC Platform é um ecossistema SaaS composto por:

- **Site público** (landing page) com conteúdo gerenciado pelo painel GDAC
- **Painel GDAC** — administração interna: conteúdo, usuários, empresas, parceiros
- **Painel do Parceiro** — gestão de clientes vinculados ao parceiro (revenda)
- **Painel do Cliente** — gestão da empresa, usuários e serviços contratados

---

## 2. Repositórios

| Repositório | Descrição |
|-------------|-----------|
| `Gdac.Solutions.2026` | Backend: Auth, Core, Onboarding, Content APIs |
| `Gdac.Platform` | Frontend: Nx monorepo com os 4 apps Angular |

---

## 3. Arquitetura Frontend — Nx Monorepo (`Gdac.Platform`)

```
Gdac.Platform/
  apps/
    landing/           → site público (Angular SSR) — conteúdo dinâmico
    portal-gdac/       → painel interno GDAC
    portal-partner/    → painel do parceiro/revenda
    portal-client/     → painel do cliente
  libs/
    ui/                → componentes visuais compartilhados
    auth/              → serviço JWT, interceptors, guards
    api/               → clients para todas as APIs backend
    shared/            → modelos, utils, validators, pipes
```

### Por que Nx?
- Builds incrementais — só recompila o app afetado pelo commit
- Bibliotecas compartilhadas entre os 4 apps sem duplicação
- CI/CD inteligente com deploy seletivo por app

---

## 4. Arquitetura Backend (`Gdac.Solutions.2026`)

| Serviço | Status | Responsabilidade |
|---------|--------|-----------------|
| `Gdac.Auth.Api` | ✅ Pronto | Autenticação: registro, login, tokens JWT RS256 |
| `Gdac.Core.Api` | ✅ Pronto | Perfis de usuário, empresas, sócios, estabelecimentos |
| `Gdac.Onboarding.Api` | 🔲 Novo | Cadastro público de clientes + código de parceiro |
| `Gdac.Content.Api` | 🔲 Novo | CMS: banners, produtos, serviços, depoimentos |

### Fluxo geral

```
Usuário público → landing          → Onboarding API → Auth API + Core API
Usuário GDAC   → portal-gdac      → Content API + Core API + Auth API
Parceiro       → portal-partner   → Core API (clientes vinculados)
Cliente        → portal-client    → Core API + Auth API
```

---

## 5. Conteúdo gerenciado pelo Painel GDAC

Tudo que aparece na landing page é configurável via `portal-gdac` e servido pelo `Gdac.Content.Api`.

| Seção | Entidades gerenciadas |
|-------|----------------------|
| Hero / Banners | Título, subtítulo, imagem, CTA, ordem, ativo/inativo |
| Carrosséis | Itens com imagem, texto, link, ordem |
| Produtos | Nome, descrição, ícone, destaque, categoria, ativo |
| Serviços | Nome, descrição, ícone, destaque, categoria, ativo |
| Depoimentos | Autor, cargo, empresa, foto, texto, ordem, ativo |
| Vitrine de Parceiros | Parceiros que aceitaram ser listados publicamente |
| Vitrine de Clientes | Clientes que aceitaram ser listados publicamente |
| Aplicações / Integrações | Nome, logo, descrição, link |

---

## 6. Sistema de Parceiros e Código de Parceiro

### Modelo de vínculo

Toda empresa cadastrada pode ter um **parceiro responsável**:

```
Company
  └── PartnerId (Guid?)     → FK para Company do tipo Partner
  └── PartnerCode (string?) → código legível usado no cadastro público
```

### Fluxo de cadastro público (Onboarding)

1. Usuário acessa a landing page e clica em "Cadastrar"
2. Preenche dados da empresa (e opcionalmente o **Código do Parceiro**)
3. Preenche dados do usuário master
4. Onboarding API:
   - Valida o código do parceiro (se informado)
   - Cria o usuário no `Gdac.Auth.Api`
   - Cria o perfil no `Gdac.Core.Api`
   - Cria a empresa no `Gdac.Core.Api` com `PartnerId` resolvido
   - Vincula o usuário à empresa como `master`
5. Usuário recebe e-mail de boas-vindas e acessa o painel

### Transferência entre parceiros

Endpoint autenticado (apenas GDAC):

```
PUT /companies/{id}/partner
Body: { "partnerId": "<guid>" | null }
```

- `partnerId: null` → empresa passa a ser cliente direto da GDAC
- Histórico de transferências pode ser registrado para auditoria

---

## 7. Perfis de usuário na plataforma

| Perfil | Onde acessa | Permissões |
|--------|-------------|-----------|
| `gdac-admin` | portal-gdac | Tudo: conteúdo, plataforma, usuários, empresas |
| `partner-admin` | portal-partner | Seus clientes, relatórios, usuários do painel |
| `partner-user` | portal-partner | Visualização dos clientes, sem gestão |
| `master` | portal-client | Gestão total da empresa e usuários |
| `admin` | portal-client | Gestão de usuários da empresa |
| `user` | portal-client | Acesso aos serviços contratados |

---

## 8. Cadastro público — o que pode ser feito sem login

| Ação | Requer login |
|------|-------------|
| Ver landing page | Não |
| Cadastrar empresa + usuário master | Não |
| Informar código de parceiro | Não |
| Atualizar dados da empresa | Sim |
| Adicionar/remover usuários | Sim |
| Acessar painéis | Sim |

---

## 9. Ordem de construção sugerida

```
Fase 1 — Gdac.Content.Api
  └── Entidades: Banner, Carousel, Product, Service, Testimonial, ShowcaseItem
  └── CRUD completo com endpoints públicos (GET) e autenticados (POST/PUT/DELETE)

Fase 2 — portal-gdac (CMS + gestão)
  └── Gerenciar conteúdo da landing
  └── Gerenciar empresas, parceiros, transferências

Fase 3 — landing (SSR)
  └── Consome Content API
  └── Formulário de cadastro público (chama Onboarding API)
  └── SEO, performance, animações

Fase 4 — Gdac.Onboarding.Api
  └── POST /onboarding/register (empresa + usuário master + código parceiro)
  └── Orquestra Auth + Core internamente com service token

Fase 5 — portal-partner
  └── Lista de clientes vinculados
  └── Relatórios, gestão de usuários do parceiro

Fase 6 — portal-client
  └── Dashboard da empresa
  └── Gestão de usuários
  └── Serviços contratados
```

---

## 10. Infraestrutura e deploy

| App | Domínio (previsto) | Tipo |
|-----|--------------------|------|
| landing | `gdac.com.br` | SSR (Node/Angular) |
| portal-gdac | `app.gdac.com.br` | SPA |
| portal-partner | `partner.gdac.com.br` | SPA |
| portal-client | `portal.gdac.com.br` | SPA |
| Onboarding API | `onboarding-api.gdac.com.br` | .NET 10 |
| Content API | `content-api.gdac.com.br` | .NET 10 |

Todos os serviços rodam no mesmo VPS KingHost via Docker + nginx, seguindo o padrão já estabelecido.

---

## 11. Pontos em aberto (a definir)

- [ ] Identidade visual: paleta de cores, tipografia, logo para o frontend
- [ ] Nome dos planos/produtos que serão promovidos na landing
- [ ] Quais integrações/aplicações serão listadas inicialmente
- [ ] Fluxo de aprovação de cadastro: automático ou requer aprovação GDAC?
- [ ] E-mail de boas-vindas: template, conteúdo
- [ ] Grupo de empresas: como modelar (empresa pai → filiais)?
- [ ] Módulo de relatórios: o que cada perfil enxerga?
- [ ] Pagamento/cobrança: faz parte da plataforma ou é externo?
- [ ] Multi-idioma (pt-BR apenas por enquanto ou previsto pt/en/es)?
- [ ] App mobile previsto ou só web?

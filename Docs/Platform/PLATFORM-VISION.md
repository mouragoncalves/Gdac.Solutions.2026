# GDAC Platform — Visão Geral

Documento vivo de decisões arquiteturais e de produto da plataforma GDAC.  
Atualizado conforme novas decisões são tomadas.

---

## 1. O que é a plataforma

A GDAC Platform é um ecossistema SaaS composto por:

- **Site público GDAC** (landing page) com conteúdo gerenciado pelo painel GDAC
- **Landing pages de parceiros** — cada parceiro tem seu subdomínio customizável
- **Painel GDAC** — administração interna: conteúdo, parceiros, clientes, financeiro, cobrança
- **Painel do Parceiro** — gestão de clientes vinculados, financeiro, cobrança, contratos
- **Painel do Cliente** — mensalidades, contratos, acesso aos produtos contratados
- **Produtos comercializados** — ControlUP e Count+ (apps independentes com SSO via Auth)

---

## 2. Repositórios

| Repositório | Descrição |
|-------------|-----------|
| `Gdac.Solutions.2026` | Backend: todos os serviços .NET 10 |
| `Gdac.Platform` | Frontend: Nx monorepo com todos os apps Angular |

---

## 3. Arquitetura Frontend — Nx Monorepo (`Gdac.Platform`)

```
Gdac.Platform/
  apps/
    landing-gdac/       → site público GDAC (Angular SSR)
    landing-partner/    → landing multi-tenant por subdomínio (Angular SSR)
    portal-gdac/        → painel interno GDAC
    portal-partner/     → painel do parceiro/revenda
    portal-client/      → painel do cliente
  libs/
    ui/                 → componentes visuais compartilhados
    auth/               → serviço JWT, interceptors, guards
    api/                → clients para todas as APIs backend
    shared/             → modelos, utils, validators, pipes
```

### Landing do parceiro — multi-tenant por subdomínio

Uma única app SSR lê o subdomínio da requisição e carrega conteúdo e tema do parceiro via Content API. A GDAC libera o subdomínio pelo painel.

```
cliente acessa → parceiro-x.gdac.com.br
                       ↓
             SSR lê subdomínio "parceiro-x"
                       ↓
             Content API retorna tema + conteúdo do parceiro
                       ↓
             Renderiza landing personalizada
```

---

## 4. Arquitetura Backend (`Gdac.Solutions.2026`)

| Serviço | Status | Responsabilidade |
|---------|--------|-----------------|
| `Gdac.Auth.Api` | ✅ Pronto | Autenticação, tokens JWT RS256 |
| `Gdac.Core.Api` | ✅ Pronto | Perfis, empresas, parceiros, vínculos, block list |
| `Gdac.Content.Api` | 🔲 Novo | CMS: landing GDAC + landing parceiros |
| `Gdac.Onboarding.Api` | 🔲 Novo | Cadastro público, distribuição de leads |
| `Gdac.Financial.Api` | 🔲 Novo | Caixa, bancos, contas a pagar, contas a receber |
| `Gdac.Billing.Api` | 🔲 Novo | Cobranças, gateway Asaas, recorrência |
| `Gdac.Contract.Api` | 🔲 Novo | Modelos de contrato, emissão, aceite digital |
| `Gdac.Notification.Api` | 🔲 Novo | E-mail (SMTP KingHost) + WhatsApp (Evolution API) |

### Produtos (repositórios independentes)

| Produto | Descrição |
|---------|-----------|
| `ControlUP` | Controle de estoque + financeiro + DRE. Integra com ERP do cliente, analisa movimentações de compra/venda, estoque ideal, giro, custo médio, CPV. |
| `Count+` | Contagem de estoque. Usa base do cliente para identificar produtos e registrar valores de contagem. |

Ambos usam SSO via `Gdac.Auth.Api`. O acesso é liberado conforme o contrato ativo do cliente.

---

## 5. Status

### Parceiro

| Status | Descrição |
|--------|-----------|
| `Prospecto` | Em negociação, ainda não ativado |
| `Ativo` | Operando normalmente |
| `Inadimplente` | Com pagamentos em atraso, ainda opera |
| `Suspenso` | Acesso bloqueado temporariamente (reversível) |
| `Desligado` | Encerramento formal do contrato |
| `Bloqueado` | Lista negra — só GDAC pode reativar |

### Cliente

| Status | Descrição |
|--------|-----------|
| `Prospecto` | Cadastrado, ainda não contratou |
| `Ativo` | Contrato vigente, pagamentos em dia |
| `Inadimplente` | Pagamentos em atraso, acesso pode ser limitado |
| `Suspenso` | Acesso bloqueado temporariamente (reversível) |
| `Cancelado` | Cancelou o contrato |
| `Bloqueado` | Lista negra — só GDAC pode reativar |

### Transições de status permitidas

| Quem | Pode fazer |
|------|-----------|
| GDAC | Qualquer transição, inclusive reativar `Bloqueado` |
| Parceiro | Pode suspender seu cliente → vai para fila de aprovação da GDAC |
| Sistema | `Ativo` → `Inadimplente` automaticamente após X dias de atraso |

---

## 6. Soft Delete e Lista Negra

Nenhum registro é excluído definitivamente. Ao encerrar ou bloquear uma empresa:

1. O status muda para `Desligado` ou `Bloqueado`
2. Um `BlockRecord` é criado com: motivo, data, responsável e snapshot dos dados principais
3. Na tentativa de novo cadastro (Onboarding), o CNPJ é consultado na block list
4. Se bloqueado: exibe alerta e impede o cadastro — nem o parceiro consegue habilitar
5. Apenas a GDAC pode reativar um registro bloqueado

```csharp
public class BlockRecord
{
    public Guid     Id            { get; }
    public Guid     CompanyId     { get; }
    public string   Reason        { get; }
    public string   CompanyName   { get; }   // snapshot
    public string   Cnpj          { get; }   // snapshot
    public Guid     BlockedBy     { get; }   // userId
    public DateTime BlockedAt     { get; }
    public string   BlockedByRole { get; }   // gdac-admin | partner-admin
}
```

---

## 7. Distribuição de Leads (cadastros sem código de parceiro)

Quando um cliente se cadastra na landing GDAC sem informar código de parceiro, o sistema aplica a regra configurada pela GDAC:

| Modo | Comportamento |
|------|--------------|
| `Manual` | Cadastro fica como `Prospecto` sem parceiro — GDAC atribui depois |
| `RevendaPadrao` | Atribuído automaticamente a uma revenda configurada |
| `Sorteio` | Sorteado entre as top 5 revendas ativas por volume de clientes |
| `Proximidade` | Atribuído ao parceiro mais próximo por cidade/estado |

- A GDAC pode ter clientes diretos (PartnerId nullable)
- O modo de distribuição é configurável no painel GDAC e pode ser alterado a qualquer momento
- Para o modo `Proximidade`, parceiros precisam ter cidade e estado cadastrados

---

## 8. Políticas de Parceria

| Política | Como funciona |
|----------|--------------|
| `Comissão` | GDAC cobra o cliente, parceiro recebe % sobre o valor |
| `Revenda` | Parceiro cobra o cliente ao preço final, paga GDAC o preço de revenda |
| `Representante` | Parceiro indica, GDAC fecha, parceiro recebe % (estrutura contratual diferente da comissão) |

A política impacta diretamente o fluxo de cobrança e os registros financeiros gerados.

---

## 9. Precificação de Produtos

Ao cadastrar e liberar um novo produto:

| Campo | Quem define | Descrição |
|-------|------------|-----------|
| `PrecoRevenda` | GDAC | Valor cobrado do parceiro |
| `PrecoSugeridoFinal` | GDAC | Sugestão de preço para o consumidor final |
| `PrecoFinal` | Parceiro | Preço que o parceiro cobra do cliente |

**Regras:**
- `PrecoFinal` mínimo = `PrecoRevenda × 1,20` (margem mínima de 20%)
- A API rejeita qualquer contratação com `PrecoFinal` abaixo do mínimo
- O parceiro pode manter o preço sugerido ou ajustar para cima

### Fluxo de cobrança na contratação

```
Parceiro define PrecoFinal para o cliente
        ↓
Sistema gera: Contrato cliente (PrecoFinal)
        ↓
Sistema gera automaticamente: Registro de cobrança GDAC (PrecoRevenda)
```

---

## 10. Contratos

- Modelos disponíveis: **Mensal**, **Semestral**, **Anual**
- Aceite digital: semelhante aos termos de uso de um app (sem assinatura ICP-Brasil)
- GDAC cria modelos globais; parceiros podem criar seus próprios modelos
- Cada contrato registra: produto, prazo, preço final, data de contratação, bonificações e validade
- O cliente visualiza seus contratos ativos no portal

---

## 11. Notificações

| Canal | Tecnologia | Uso |
|-------|-----------|-----|
| E-mail | SMTP KingHost (`smtp.kinghost.net:465`) | Boas-vindas, cobranças, alertas |
| WhatsApp | Evolution API (self-hosted, open source) | Lembretes de pagamento, notificações importantes |

Toda comunicação é gerenciada pelo `Gdac.Notification.Api` e acionada pelos demais serviços via eventos.

---

## 12. Módulos por Painel

### Portal GDAC (`portal-gdac`)

| Módulo | Funcionalidades |
|--------|----------------|
| **Dashboard** | Visão geral financeira, parceiros em dia vs inadimplentes, top 10 melhores/piores parceiros por clientes e repasse, alertas críticos |
| **Parceiros** | Cadastro, status, política de parceria, subdomínio, distribuição de leads, clientes vinculados, transferências |
| **Clientes** | Visão de todos os clientes, status, vínculos com parceiros |
| **Financeiro** | Caixa, bancos, contas a pagar, contas a receber (GDAC) |
| **Cobrança** | Geração de cobranças para parceiros e clientes diretos, status de pagamentos, gateway Asaas |
| **Contratos** | Modelos globais, emissão, histórico |
| **Produtos** | Cadastro, precificação, liberação de acesso |
| **Conteúdo** | CMS da landing GDAC |
| **Block List** | Visualização e gestão de registros bloqueados |
| **Leads** | Configuração do modo de distribuição, fila de leads sem parceiro |

### Portal Parceiro (`portal-partner`)

| Módulo | Funcionalidades |
|--------|----------------|
| **Dashboard** | Clientes em dia vs inadimplentes, top clientes por valor, visão do financeiro do parceiro |
| **Clientes** | Lista, cadastro, status, suspensão (com aprovação GDAC) |
| **Financeiro** | Contas a receber (comissões ou faturas de revenda), extrato de repasses para GDAC |
| **Cobrança** | Cobranças dos seus clientes (se política revenda), status de pagamentos |
| **Contratos** | Modelos próprios, emissão para clientes |
| **Landing** | Customização da landing page do parceiro |

### Portal Cliente (`portal-client`)

| Módulo | Funcionalidades |
|--------|----------------|
| **Dashboard** | Resumo de contratos ativos, próximos vencimentos |
| **Financeiro** | Mensalidades, status, upload de comprovante, escolha de forma de pagamento (PIX/boleto/cartão) |
| **Contratos** | Visualização de contratos, data de contratação, bonificações, validade |
| **Produtos** | Acesso ao ControlUP e/ou Count+ conforme contratos ativos |

---

## 13. Dashboards — Atualização de Dados

- **Atualização automática:** a cada 5 minutos (polling no frontend)
- **Atualização manual:** botão de refresh por seção e botão geral de refresh total
- **Performance:** job noturno agrega métricas em tabela de resumo; dashboard lê o resumo, não calcula em tempo real
- **Dados do dashboard GDAC:** financeiro geral, parceiros inadimplentes, top 10 melhores/piores revendas (por clientes e valores de repasse), alertas de bloqueio pendentes, leads sem parceiro
- **Dados do dashboard Parceiro:** financeiro do parceiro, clientes inadimplentes, top clientes, solicitações de suspensão pendentes

---

## 14. Infraestrutura e Domínios

| App / Serviço | Domínio previsto | Tipo |
|---------------|-----------------|------|
| `landing-gdac` | `gdac.com.br` | SSR (Angular) |
| `landing-partner` | `*.gdac.com.br` (subdomínio por parceiro) | SSR (Angular) |
| `portal-gdac` | `app.gdac.com.br` | SPA |
| `portal-partner` | `partner.gdac.com.br` | SPA |
| `portal-client` | `portal.gdac.com.br` | SPA |
| `Gdac.Onboarding.Api` | `onboarding-api.gdac.com.br` | .NET 10 |
| `Gdac.Content.Api` | `content-api.gdac.com.br` | .NET 10 |
| `Gdac.Financial.Api` | `financial-api.gdac.com.br` | .NET 10 |
| `Gdac.Billing.Api` | `billing-api.gdac.com.br` | .NET 10 |
| `Gdac.Contract.Api` | `contract-api.gdac.com.br` | .NET 10 |
| `Gdac.Notification.Api` | `notification-api.gdac.com.br` | .NET 10 |

Todos rodam no mesmo VPS KingHost via Docker + nginx, seguindo o padrão já estabelecido.

---

## 15. Gateway de Pagamento

**Asaas** — integrado via `Gdac.Billing.Api`
- Boleto, PIX, cartão de crédito
- Assinaturas recorrentes (mensal/semestral/anual)
- Webhooks para confirmação automática de pagamento e atualização de status

---

## 16. Ordem de Construção

```
Fase 1 — Base de conteúdo
  Gdac.Content.Api (entidades: Banner, Carousel, Product, Service, Testimonial, ShowcaseItem)
  portal-gdac → módulo CMS
  landing-gdac (SSR, consome Content API)

Fase 2 — Parceiros e onboarding
  Gdac.Core.Api: campos de parceria (política, status, subdomínio, localização, BlockRecord)
  Gdac.Onboarding.Api (cadastro público + block list check + distribuição de leads)
  landing-partner (multi-tenant SSR)
  portal-gdac → módulo Parceiros
  portal-partner → módulo Clientes + Landing customizada

Fase 3 — Financeiro e cobrança
  Gdac.Financial.Api (caixa, bancos, CP, CR)
  Gdac.Billing.Api (Asaas: boleto, PIX, cartão, recorrência)
  Gdac.Contract.Api (modelos, emissão, aceite digital)
  portal-gdac → módulos Financeiro, Cobrança, Contratos, Dashboard
  portal-partner → módulos Financeiro, Cobrança, Contratos, Dashboard
  portal-client → módulos Financeiro, Contratos

Fase 4 — Notificações
  Gdac.Notification.Api (SMTP + Evolution API WhatsApp)
  Integração com todos os serviços que geram eventos de notificação

Fase 5 — Produtos
  ControlUP (repositório independente, SSO via Auth)
  Count+ (repositório independente, SSO via Auth)
  portal-client → módulo Produtos
  portal-gdac → módulo Produtos (gestão de licenças)
```

---

## 17. Pontos em Aberto

- [ ] Identidade visual: paleta, tipografia, logo para os frontends
- [ ] Fluxo de aprovação de cadastro: automático (`Ativo`) ou revisão (`Prospecto` → GDAC ativa)?
- [ ] Após quantos dias de atraso o sistema muda `Ativo` → `Inadimplente`?
- [ ] Desconto por prazo de contrato: semestral e anual têm desconto? Qual %?
- [ ] Quais produtos são listados inicialmente na landing?
- [ ] Quais integrações/apps aparecem na vitrine?
- [ ] Multi-idioma: pt-BR apenas ou previsto pt/en/es?
- [ ] App mobile previsto ou só web?
- [ ] Número WhatsApp Business: já tem conta aprovada pela Meta?
- [ ] Critério exato do top 5 para sorteio de leads: por clientes ativos? Por volume de repasse?

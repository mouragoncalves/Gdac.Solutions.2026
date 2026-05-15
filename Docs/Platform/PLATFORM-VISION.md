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
- **Produtos comercializados** — ControlUP e Count+ (apps independentes, fora do escopo atual)

---

## 2. Repositórios

| Repositório | Descrição |
|-------------|-----------|
| `Gdac.Solutions.2026` | Backend: todos os serviços .NET 10 |
| `Gdac.Platform` | Frontend: Nx monorepo com todos os apps Angular |

---

## 3. Identidade Visual

### Framework
**Angular Material** (Material Design Google)

### Responsividade

Todos os apps são **mobile-first** e responsivos em todos os breakpoints:

| Breakpoint | Largura | Comportamento |
|------------|---------|--------------|
| `xs` | < 600px | Layout mobile: sidebar vira drawer, tabelas viram cards, modais viram bottom sheets |
| `sm` | 600–959px | Tablet: 2 colunas, navbar compacta |
| `md` | 960–1279px | Desktop pequeno: sidebar collapsível |
| `lg` | 1280–1919px | Desktop padrão: layout completo |
| `xl` | ≥ 1920px | Desktop grande: conteúdo centralizado com max-width |

### Paleta de cores

| Token | Claro | Escuro | Uso |
|-------|-------|--------|-----|
| Primary | `#1565C0` (Blue 800) | `#42A5F5` (Blue 300) | Ações principais, navegação |
| Accent | `#00BFA5` (Teal A700) | `#1DE9B6` (Teal A400) | Destaques, CTAs secundários |
| Warn | `#D32F2F` | `#EF9A9A` | Erros, alertas críticos |
| Background | `#FAFAFA` | `#121212` | Fundo da aplicação |
| Surface | `#FFFFFF` | `#1E1E1E` | Cards, painéis |

### Tipografia
**Inter** — moderna, legível, profissional. Disponível via Google Fonts.

### Logo
- Gerenciada via upload no painel GDAC (mesmo mecanismo do parceiro)
- Arquivo de referência: `Docs/Assets/logo/` (colocar os arquivos nessa pasta)
- Parceiro importa sua própria logo pelo portal-partner

### Tema
Suporte nativo a tema **claro e escuro** via Angular Material theming. O usuário pode alternar pelo painel.

---

## 4. Arquitetura Frontend — Nx Monorepo (`Gdac.Platform`)

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

### Apps mobile (escopo futuro — Flutter)

Apps distintos, desenvolvidos separadamente após a conclusão dos portais web:

| App | Plataforma | Usuário |
|-----|-----------|---------|
| `gdac-partner-mobile` | Flutter (iOS + Android) | Parceiros |
| `gdac-client-mobile` | Flutter (iOS + Android) | Clientes |

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

## 5. Arquitetura Backend (`Gdac.Solutions.2026`)

| Serviço | Status | Responsabilidade |
|---------|--------|-----------------|
| `Gdac.Auth.Api` | ✅ Pronto | Autenticação, tokens JWT RS256 |
| `Gdac.Core.Api` | ✅ Pronto | Perfis, empresas, parceiros, vínculos, block list |
| `Gdac.Content.Api` | 🔲 Novo | CMS: landing GDAC + landing parceiros + catálogo de produtos/serviços |
| `Gdac.Onboarding.Api` | 🔲 Novo | Cadastro público, distribuição de leads |
| `Gdac.Financial.Api` | 🔲 Novo | Caixa, bancos, contas a pagar, contas a receber |
| `Gdac.Billing.Api` | 🔲 Novo | Cobranças, gateway Asaas, recorrência |
| `Gdac.Contract.Api` | 🔲 Novo | Modelos de contrato, emissão, aceite digital |
| `Gdac.Notification.Api` | 🔲 Novo | E-mail (SMTP KingHost) + WhatsApp (Evolution API) |

### Produtos (repositórios independentes — fora do escopo atual)

| Produto | Descrição |
|---------|-----------|
| `ControlUP` | Controle de estoque + financeiro + DRE. Integra com ERP do cliente, analisa movimentações de compra/venda, estoque ideal, giro, custo médio, CPV. |
| `Count+` | Contagem de estoque. Usa base do cliente para identificar produtos e registrar valores de contagem. |

Ambos usam SSO via `Gdac.Auth.Api`. O acesso é liberado conforme o contrato ativo do cliente.

---

## 6. Status

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
| Sistema | `Ativo` → `Inadimplente` automaticamente a partir do **5º dia corrido** de atraso |

### Fluxo de aprovação de cadastro

Todo novo cadastro inicia com status **`Prospecto`**. A ativação é manual — feita pela GDAC ou pelo parceiro responsável.

---

## 7. Soft Delete e Lista Negra

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

## 8. Distribuição de Leads (cadastros sem código de parceiro)

Quando um cliente se cadastra na landing GDAC sem informar código de parceiro, o sistema aplica a regra configurada pela GDAC:

| Modo | Comportamento |
|------|--------------|
| `Manual` | Cadastro fica como `Prospecto` sem parceiro — GDAC atribui depois |
| `RevendaPadrao` | Atribuído automaticamente a uma revenda configurada |
| `Sorteio` | Sorteado entre as top 5 revendas por volume total de repasse; empate desfeito por quantidade de clientes ativos |
| `Proximidade` | Atribuído ao parceiro mais próximo por cidade/estado |

- A GDAC pode ter clientes diretos (PartnerId nullable)
- O modo de distribuição é configurável no painel GDAC e pode ser alterado a qualquer momento
- Para o modo `Proximidade`, parceiros precisam ter cidade e estado cadastrados
- Para o modo `Sorteio`, apenas parceiros com status `Ativo` participam

---

## 9. Políticas de Parceria

| Política | Como funciona |
|----------|--------------|
| `Comissão` | GDAC cobra o cliente, parceiro recebe % sobre o valor |
| `Revenda` | Parceiro cobra o cliente ao preço final, paga GDAC o preço de revenda |
| `Representante` | Parceiro indica, GDAC fecha, parceiro recebe % (estrutura contratual diferente da comissão) |

A política impacta diretamente o fluxo de cobrança e os registros financeiros gerados.

---

## 10. Cadastro de Produtos e Serviços

Produtos e serviços só aparecem na landing quando cadastrados e ativos no painel GDAC. O mesmo vale para integrações/apps da vitrine.

### Campos do cadastro

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Nome | texto | Nome do produto/serviço |
| Categoria | enum | Classificação |
| Texto de apresentação | rich text | Descrição completa para landing |
| Fotos | múltiplos arquivos | Imagens do produto/serviço |
| Vídeos | múltiplos arquivos / URL | Demonstrações |
| Ativo | boolean | Exibir ou não na landing |
| Ordem | inteiro | Posição na listagem |
| `PrecoRevenda` | decimal | Valor mensal cobrado do parceiro |
| `PrecoSugeridoFinal` | decimal | Sugestão de preço mensal para o cliente |
| `DescontoSugeridoSemestral` | % | Desconto sugerido no plano semestral — padrão **10%** |
| `ValorSemestralRevenda` | calculado | `PrecoRevenda × 6 × (1 − DescontoSugeridoSemestral)` |
| `ValorSugeridoSemestralFinal` | calculado | Valor semestral sugerido ao cliente |
| `DescontoSugeridoAnual` | % | Desconto sugerido no plano anual — padrão **25%** |
| `ValorAnualRevenda` | calculado | `PrecoRevenda × 12 × (1 − DescontoSugeridoAnual)` |
| `ValorSugeridoAnualFinal` | calculado | Valor anual sugerido ao cliente |

### Histórico de preços

Cada alteração nos valores gera um registro de histórico:

```csharp
public class ProductPriceHistory
{
    public Guid     Id                          { get; }
    public Guid     ProductId                   { get; }
    public decimal  PrecoRevenda                { get; }
    public decimal  PrecoSugeridoFinal          { get; }
    public decimal  DescontoSugeridoSemestral   { get; }
    public decimal  DescontoSugeridoAnual       { get; }
    public Guid     ChangedBy                   { get; }
    public DateTime ChangedAt                   { get; }
    public string?  Notes                       { get; }
}
```

### Precificação pelo parceiro

O parceiro define seus próprios valores ao disponibilizar o produto para seus clientes:

| Campo | Regra |
|-------|-------|
| `PrecoFinal` (mensal) | Mínimo: `PrecoRevenda × 1,20` |
| `DescontoSemestral` | Pode ser maior que o sugerido; mínimo: `PrecoFinal semestral ≥ PrecoRevenda × 1,20` |
| `DescontoAnual` | Mesma regra do semestral |

A API rejeita qualquer valor abaixo do piso de margem. O parceiro pode aumentar sem restrição de teto.

### Fluxo de cobrança na contratação

```
Parceiro define PrecoFinal + descontos
        ↓
Cliente contrata (mensal / semestral / anual)
        ↓
Sistema gera: Contrato do cliente (PrecoFinal no prazo escolhido)
        ↓
Sistema gera automaticamente: Registro de cobrança GDAC (PrecoRevenda no mesmo prazo)
```

---

## 11. Vitrine de Integrações

Integrações e apps parceiros só aparecem na landing quando cadastrados. Categorias disponíveis:

| Categoria | Exemplos |
|-----------|---------|
| ERP / Gestão | TOTVS, SAP Business One, Bling, Omie, Linx, Senior |
| E-commerce | Shopify, VTEX, WooCommerce, Nuvemshop |
| Marketplace | Mercado Livre, Amazon, Shopee |
| Fiscal / NF-e | NFe.io, Focus NF-e, SEFAZ |
| Financeiro | Conta Azul, Sicoob, Open Banking |
| PDV / Pagamentos | Stone, Cielo, PagSeguro, Asaas |
| Logística | Correios, Jadlog, Total Express |
| BI / Analytics | Power BI, Google Looker Studio |

Cada integração tem: nome, logo, descrição, categoria, link externo, ordem e ativo/inativo.

---

## 12. Contratos

- Modelos disponíveis: **Mensal**, **Semestral**, **Anual**
- Aceite digital: semelhante aos termos de uso de um app (sem assinatura ICP-Brasil)
- GDAC cria modelos globais; parceiros podem criar seus próprios modelos
- Cada contrato registra: produto, prazo, preço final, data de contratação, bonificações e validade
- O cliente visualiza seus contratos ativos no portal

---

## 13. Notificações

| Canal | Tecnologia | Configuração |
|-------|-----------|-------------|
| E-mail | SMTP KingHost (`smtp.kinghost.net:465`) | Centralizado no `Gdac.Notification.Api` |
| WhatsApp | Evolution API (self-hosted, open source, multi-instância) | Cada entidade configura seu próprio número |

### WhatsApp por entidade

- **GDAC**: configura seu número no portal-gdac
- **Parceiro**: configura seu próprio número no portal-partner
- Cada número é uma instância separada no Evolution API — nunca compartilhadas
- Evolution API roda no mesmo VPS via Docker

Toda comunicação é gerenciada pelo `Gdac.Notification.Api` e acionada pelos demais serviços via eventos.

---

## 14. Módulos por Painel

### Portal GDAC (`portal-gdac`)

| Módulo | Funcionalidades |
|--------|----------------|
| **Dashboard** | Financeiro geral, parceiros em dia vs inadimplentes, top 10 melhores/piores por clientes e repasse, leads pendentes, alertas de bloqueio, refresh por seção + geral + auto a cada 5 min |
| **Parceiros** | Cadastro, status, política, subdomínio, distribuição de leads, transferências, WhatsApp |
| **Clientes** | Todos os clientes, status, vínculos, histórico |
| **Financeiro** | Caixa, bancos, contas a pagar, contas a receber |
| **Cobrança** | Cobranças para parceiros e clientes diretos, status, gateway Asaas |
| **Contratos** | Modelos globais, emissão, histórico |
| **Produtos / Serviços** | Cadastro rico (fotos, vídeos, texto), precificação, histórico de preços, ativo/inativo |
| **Integrações** | Vitrine de apps e integrações por categoria |
| **Conteúdo** | CMS da landing GDAC (banners, carrosséis, depoimentos, parceiros em destaque) |
| **Block List** | Visualização e reativação de registros bloqueados |
| **Leads** | Modo de distribuição, fila de leads sem parceiro |
| **Configurações** | Logo, WhatsApp, dados da empresa, tema |

### Portal Parceiro (`portal-partner`)

| Módulo | Funcionalidades |
|--------|----------------|
| **Dashboard** | Clientes em dia vs inadimplentes, top clientes por valor, financeiro do parceiro, refresh por seção + geral + auto a cada 5 min |
| **Clientes** | Lista, cadastro, status, suspensão (aprovação GDAC), histórico |
| **Financeiro** | Contas a receber, extrato de repasses para GDAC |
| **Cobrança** | Cobranças dos clientes (política revenda), status de pagamentos |
| **Contratos** | Modelos próprios, emissão para clientes, precificação por produto |
| **Produtos / Serviços** | Visualização do catálogo GDAC, definição de PrecoFinal e descontos |
| **Landing** | Customização da landing page (logo, cores, textos, produtos em destaque) |
| **Configurações** | WhatsApp, dados do parceiro |

### Portal Cliente (`portal-client`)

| Módulo | Funcionalidades |
|--------|----------------|
| **Dashboard** | Contratos ativos, próximos vencimentos, status financeiro |
| **Financeiro** | Mensalidades, status, upload de comprovante, escolha de pagamento (PIX/boleto/cartão) |
| **Contratos** | Visualização de contratos, data, bonificações, validade |
| **Produtos** | Acesso aos produtos contratados (ControlUP, Count+ via SSO) |

---

## 15. Dashboards — Atualização de Dados

- **Atualização automática:** a cada 5 minutos (polling no frontend)
- **Atualização manual:** botão de refresh por seção e botão geral de refresh total
- **Performance:** job noturno agrega métricas em tabela de resumo; dashboard lê o resumo, não calcula em tempo real

---

## 16. Infraestrutura e Domínios

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
| `Evolution API` | `whatsapp-api.gdac.com.br` | Docker (self-hosted) |

Todos rodam no mesmo VPS KingHost via Docker + nginx.

---

## 17. Gateway de Pagamento

**Asaas** — integrado via `Gdac.Billing.Api`
- Boleto, PIX, cartão de crédito
- Assinaturas recorrentes (mensal/semestral/anual)
- Webhooks para confirmação automática de pagamento e atualização de status

---

## 18. Multi-idioma

- **Atual:** pt-BR
- **Futuro:** inglês (en)
- A estrutura de i18n será implementada desde a **Fase 1** usando **ngx-translate** — todos os textos já passam por chave de tradução desde o primeiro componente criado

---

## 19. Ordem de Construção

```
Fase 1 — Base de conteúdo
  Gdac.Content.Api (Banner, Carousel, Product, Service, Testimonial, Integration, ShowcaseItem)
  portal-gdac → módulo CMS + módulo Produtos/Serviços + módulo Integrações
  landing-gdac (SSR, consome Content API)

Fase 2 — Parceiros e onboarding
  Gdac.Core.Api: campos de parceria (política, status, subdomínio, localização, BlockRecord)
  Gdac.Onboarding.Api (cadastro público + block list check + distribuição de leads)
  landing-partner (multi-tenant SSR)
  portal-gdac → módulo Parceiros + Leads + Block List
  portal-partner → módulo Clientes + Landing customizada + Produtos

Fase 3 — Financeiro e cobrança
  Gdac.Financial.Api (caixa, bancos, CP, CR)
  Gdac.Billing.Api (Asaas: boleto, PIX, cartão, recorrência)
  Gdac.Contract.Api (modelos, emissão, aceite digital)
  portal-gdac → Financeiro, Cobrança, Contratos, Dashboard
  portal-partner → Financeiro, Cobrança, Contratos, Dashboard
  portal-client → Financeiro, Contratos

Fase 4 — Notificações
  Gdac.Notification.Api (SMTP + Evolution API WhatsApp multi-instância)
  Integração com todos os serviços que geram eventos

Fase 5 — Produtos (escopo futuro)
  ControlUP (repositório independente, SSO via Auth)
  Count+ (repositório independente, SSO via Auth)
  gdac-partner-mobile (Flutter)
  gdac-client-mobile (Flutter)
```

---

## 20. Pontos em Aberto

Todos os pontos foram definidos. Nenhuma decisão pendente no momento.

# GDAC Platform — Design Brief

Script completo para geração de layouts no claude.ai/design.  
Copie e cole cada seção conforme a tela que deseja criar.

---

## Prompt base — cole sempre junto com o prompt específico da tela

```
Contexto da marca:
- Empresa: GDAC — plataforma SaaS B2B brasileira de gestão empresarial
- Tom: profissional, moderno, sério, confiável. Não infantil, não pesado
- Framework: Angular Material (Material Design Google)
- Tipografia: Inter (Google Fonts) — weights 400, 500, 600, 700
- Tema: suporte a claro e escuro (gere na versão CLARA)

Paleta de cores:
- Primary: #1565C0 (Blue 800)
- Primary light: #42A5F5 (Blue 300) — usar em hovers e destaques sutis
- Accent: #00BFA5 (Teal A700)
- Accent dark: #00897B
- Warn/Error: #D32F2F
- Success: #2E7D32
- Warning: #F57F17
- Background: #FAFAFA
- Surface (cards): #FFFFFF
- Text primary: #1A1A2E
- Text secondary: #546E7A
- Divider: #E0E0E0
- Sombra padrão: 0 2px 8px rgba(0,0,0,0.08)

Princípios de UX:
- Espaçamento generoso (mínimo 16px entre elementos, 32px entre seções)
- Hierarquia visual clara: H1 > H2 > body, sem ruído
- Feedback visual em toda interação (hover, focus, loading, success, error)
- Nunca use mais de 3 cores principais por tela
- Ícones: Material Icons
- Bordas arredondadas: 8px padrão, 12px em cards, 24px em chips e badges
- RESPONSIVO: mobile-first, breakpoints em 600px, 960px, 1280px, 1920px
```

---

## 1. Landing Page GDAC — Hero com Parallax

```
Crie o layout completo da landing page da GDAC com as seguintes seções, 
usando o contexto de marca acima:

SEÇÃO 1 — NAVBAR FIXA
- Logo GDAC à esquerda (placeholder retangular azul com texto "GDAC")
- Links centrais: Produtos, Serviços, Parceiros, Integrações, Contato
- Botões à direita: "Entrar" (outlined) e "Começar grátis" (filled accent)
- Navbar transparente sobre o hero, vira branca/sólida ao rolar (scrolled state)
- Em mobile: hamburger menu com drawer lateral

SEÇÃO 2 — HERO COM PARALLAX
- Fundo com gradiente diagonal: #1565C0 → #0D47A1 com overlay sutil de partículas ou malha geométrica animada
- Headline grande (H1): "Gerencie sua empresa com inteligência"
- Subheadline (body large): "Plataforma completa para controle de estoque, financeiro e gestão de parceiros"
- Dois CTAs: "Começar agora" (accent filled, grande) e "Ver demonstração" (outlined branco)
- Imagem ou mockup de dashboard flutuando à direita com sombra e leve rotação 3D
- Efeito parallax: o mockup sobe mais devagar que o scroll
- Em mobile: mockup abaixo do texto, sem parallax

SEÇÃO 3 — NÚMEROS / PROVA SOCIAL
- Fundo branco, 4 counters animados em linha: "500+ Empresas", "98% Satisfação", "12 Integrações", "24/7 Suporte"
- Separados por linha vertical suave
- Em mobile: grid 2x2

SEÇÃO 4 — PRODUTOS E SERVIÇOS
- Título de seção centralizado: "Nossos Produtos"
- Grid de cards (3 colunas desktop, 1 mobile): cada card tem ícone Material, título, descrição curta, lista de 3 benefícios com checkmarks accent, botão "Saiba mais"
- Hover no card: elevação de sombra + borda accent sutil
- Badge "Destaque" em accent no canto superior de cards marcados

SEÇÃO 5 — COMO FUNCIONA (STEPS)
- Fundo #F5F7FA (levemente acinzentado)
- Timeline horizontal em desktop (3 steps), vertical em mobile
- Steps: 1. Cadastre sua empresa → 2. Escolha seus produtos → 3. Comece a usar
- Cada step: número em círculo accent, título, descrição curta, ícone

SEÇÃO 6 — PARCEIROS EM DESTAQUE
- Título: "Nossos Parceiros"
- Carrossel automático (loop) com logos de parceiros em cards brancos com sombra leve
- Velocidade suave, pausa no hover
- Placeholder: 8 logos com nome fictício

SEÇÃO 7 — DEPOIMENTOS
- Fundo com gradiente suave azul muito claro (#E3F2FD → #FAFAFA)
- Carrossel manual com dots de navegação
- Cada slide: foto em círculo, nome, cargo, empresa, texto do depoimento em itálico, estrelas accent
- Setas de navegação laterais

SEÇÃO 8 — INTEGRAÇÕES
- Grid 4x2 (desktop) de cards de integração: logo, nome, categoria chip
- Hover: escala 1.05, sombra accent
- Botão "Ver todas as integrações" centralizado abaixo

SEÇÃO 9 — CTA FINAL
- Fundo gradiente accent (#00BFA5 → #00897B)
- Texto branco: "Pronto para transformar sua gestão?"
- Dois botões brancos: "Falar com especialista" e "Cadastrar empresa"

SEÇÃO 10 — FOOTER
- Fundo #1A237E (azul escuro)
- 4 colunas: Logo+descrição, Links úteis, Produtos, Contato (e-mail + WhatsApp + endereço)
- Linha de copyright com links de Termos e Privacidade
- Em mobile: colunas empilhadas

Mostre o layout completo scrollável, com estados de hover visíveis nos cards e botões.
Inclua a versão mobile da navbar e do hero.
```

---

## 2. Formulário de Cadastro — Modal / Página

```
Crie um formulário de cadastro de nova empresa para a plataforma GDAC,
usando o contexto de marca acima. Pode ser modal (desktop) ou página full (mobile).

CABEÇALHO
- Logo GDAC + título "Cadastrar empresa" + subtítulo "Leva menos de 5 minutos"
- Progress bar em 3 steps: "Empresa" → "Responsável" → "Confirmar"

STEP 1 — DADOS DA EMPRESA
- Campo: Razão Social (required)
- Campo: Nome Fantasia
- Campo: CNPJ (com máscara e validação visual: loading → ✓ verde ou ✗ vermelho)
- Campo: Segmento (dropdown com ícone)
- Campo: Telefone
- Campo: Código do parceiro (opcional, com tooltip explicativo "?")
- Botão "Continuar" (accent, full width)

STEP 2 — USUÁRIO MASTER
- Campo: Nome completo
- Campo: E-mail corporativo
- Campo: Senha + Confirmar senha (com toggle show/hide)
- Campo: Telefone/WhatsApp
- Checkbox: "Aceito os termos de uso e política de privacidade" (link sublinhado)
- Botão "Continuar" + botão "Voltar" (ghost)

STEP 3 — CONFIRMAÇÃO
- Resumo dos dados em cards de leitura
- Alerta info (azul) com ícone: "Seu cadastro será revisado em até 24h"
- Botão "Confirmar cadastro" (accent, grande)

ESTADOS DE ERRO
- Mostre um campo com erro (borda vermelha + mensagem abaixo)
- Mostre o alerta de CNPJ bloqueado: card vermelho com ícone de bloqueio, texto "Este CNPJ está impedido de realizar novos cadastros. Entre em contato com o suporte."

ESTADOS DE SUCESSO
- Tela final: ícone de check em círculo accent animado, título "Cadastro realizado!", 
  subtítulo com instrução, botão "Ir para o painel"

Mostre desktop (modal centrado 640px) e mobile (full screen).
```

---

## 3. Portal GDAC — Dashboard

```
Crie o layout do dashboard principal do painel administrativo da GDAC,
usando o contexto de marca acima.

ESTRUTURA GERAL
- Sidebar fixa à esquerda (240px desktop, collapsível, drawer em mobile)
- Topbar fixa: título da página, botão de refresh geral, avatar do usuário, toggle tema claro/escuro
- Área de conteúdo com padding 24px

SIDEBAR
- Logo GDAC no topo
- Menu com ícones Material e labels:
  Dashboard, Parceiros, Clientes, Financeiro, Cobrança, Contratos, Produtos/Serviços, Integrações, Conteúdo, Block List, Leads, Configurações
- Item ativo: fundo accent suave, texto primary bold, borda esquerda accent 3px
- Hover: fundo #F5F7FA

DASHBOARD — KPI CARDS (linha superior)
- 4 cards em grid: "Parceiros Ativos" (número + variação +X% verde), 
  "Clientes Ativos", "Receita do Mês" (R$ formatado), "Inadimplência" (número + % warn)
- Cada card: ícone Material à direita em círculo colorido suave, botão de refresh individual no canto

DASHBOARD — GRÁFICOS (segunda linha)
- Gráfico de barras (60% largura): "Repasse por Parceiro — Top 10" — barras horizontais, 
  primary e accent alternados, valores formatados em R$
- Gráfico de rosca (40% largura): "Status dos Parceiros" — fatias coloridas por status, 
  legenda lateral

DASHBOARD — TABELAS (terceira linha)
- "Top 10 Parceiros" (60%): tabela com colunas Parceiro, Clientes, Repasse, Status chip colorido
- "Alertas" (40%): lista de alertas recentes com ícone, mensagem, tempo e chip de tipo 
  (Inadimplência warn, Bloqueio error, Lead info)

INDICADOR DE ATUALIZAÇÃO
- Texto sutil no topbar: "Atualizado há 2 min" + spinner quando loading
- Contador regressivo até próxima atualização automática (5 min)

Mostre layout desktop completo. Inclua versão mobile com sidebar como drawer.
```

---

## 4. Portal GDAC — Cadastro de Produto/Serviço

```
Crie o layout do formulário de cadastro de produto/serviço no painel GDAC,
usando o contexto de marca acima.

LAYOUT
- Página full dentro do painel (sidebar + topbar conforme item 3)
- Formulário em duas colunas (desktop): coluna principal (60%) + coluna de preview (40%)
- Em mobile: coluna única

COLUNA PRINCIPAL — FORMULÁRIO

Seção "Informações gerais" (card):
- Campo: Nome do produto/serviço
- Campo: Categoria (select com chips de categoria)
- Campo: Ativo/Inativo (toggle)
- Campo: Ordem de exibição (number input)

Seção "Apresentação" (card):
- Editor rich text simplificado (bold, italic, lista, link) para texto de apresentação
- Upload múltiplo de fotos: área de drag-and-drop com preview em grid 3x, 
  ícone de remover em hover, indicador de progresso de upload
- Upload múltiplo de vídeos ou URLs: tabs "Upload" / "URL", preview thumbnail

Seção "Precificação" (card com destaque):
- Campo: Preço de revenda mensal (R$ com máscara)
- Campo: Preço sugerido ao cliente (R$ calculado, editável)
- Divider
- Linha semestral: % desconto (default 10%) → valor revenda calculado → valor cliente calculado
- Linha anual: % desconto (default 25%) → valor revenda calculado → valor cliente calculado
- Todos os valores calculados atualizam em tempo real ao mudar os campos base
- Alerta info: "O parceiro pode ajustar o preço final, respeitando a margem mínima de 20%"

COLUNA DE PREVIEW
- Card "Preview na landing" mostrando como o produto aparecerá:
  miniatura com foto, nome, preço mensal, badge "Destaque" se marcado
- Atualiza em tempo real com os dados do formulário

HISTÓRICO DE PREÇOS (abaixo do formulário, card expansível)
- Tabela: Data, Preço revenda, Sugerido, Desc. Semestral, Desc. Anual, Alterado por
- Última linha em destaque (atual)

AÇÕES
- Botão "Salvar" (accent) + "Cancelar" (ghost) no rodapé fixo

Mostre o formulário preenchido com dados fictícios. Destaque os campos calculados 
em tempo real com fundo levemente accent.
```

---

## 5. Portal Parceiro — Dashboard

```
Crie o layout do dashboard do painel do parceiro na plataforma GDAC,
usando o contexto de marca acima. Tom mais caloroso que o GDAC, mas ainda profissional.

ESTRUTURA: mesma sidebar/topbar do item 3, adaptada para o parceiro.
Menu: Dashboard, Meus Clientes, Financeiro, Cobrança, Contratos, Produtos, Minha Landing, Configurações

KPI CARDS
- Clientes Ativos, Clientes Inadimplentes (warn), Receita do Mês, Próximo repasse GDAC (data + valor)

GRÁFICOS
- Linha temporal (70%): "Crescimento de clientes — últimos 12 meses"
- Mini rosca (30%): "Status dos clientes" com legenda

TABELAS
- "Clientes em atraso" (50%): nome, dias em atraso, valor, botão de ação rápida "Notificar"
- "Últimas contratações" (50%): cliente, produto, data, valor, status chip

ALERTA DE PENDÊNCIA
- Se houver suspensão pendente de aprovação GDAC: banner warn no topo da área de conteúdo
  com ícone, texto e botão "Ver detalhes"

Mostre layout desktop e indique o componente de refresh (por seção e geral).
```

---

## 6. Portal Cliente — Área Financeira

```
Crie o layout da área financeira do portal do cliente na plataforma GDAC,
usando o contexto de marca acima. Tom mais simples e amigável.

ESTRUTURA
- Topbar com logo, nome da empresa, avatar, sair
- Menu horizontal (tabs) ou sidebar simples: Dashboard, Financeiro, Contratos, Produtos

PÁGINA FINANCEIRA

Card de resumo (topo):
- Status da assinatura (chip: "Em dia" verde / "Vencida" vermelho / "Pendente" warn)
- Próximo vencimento: data + valor
- Parceiro responsável: nome + avatar

Lista de mensalidades (tabela):
- Colunas: Competência, Produto, Valor, Vencimento, Status, Comprovante, Ação
- Status como chips coloridos: Pago (verde), Pendente (warn), Vencido (erro)
- Linha com status "Pendente": botão "Pagar agora" em accent
- Linha com status "Pago": ícone de comprovante clicável

MODAL "Escolher forma de pagamento":
- Título: "Pagar R$ 297,00 — Junho/2026"
- 3 opções em cards seleccionáveis: PIX (ícone + "Aprovação imediata"), 
  Boleto (ícone + "Vence em 3 dias úteis"), Cartão de Crédito (ícone + "Parcele em até 12x")
- Card selecionado: borda accent, check mark
- Para PIX selecionado: área com QR Code placeholder + código copia-e-cola
- Para Boleto: botão "Gerar boleto"
- Para Cartão: formulário de dados do cartão

UPLOAD DE COMPROVANTE:
- Botão "Enviar comprovante" abre modal com área de drag-and-drop,
  preview da imagem e botão confirmar

Mostre desktop e mobile. No mobile o modal de pagamento deve ser bottom sheet.
```

---

## 7. Componentes de Alerta e Feedback

```
Crie uma página de showcase de todos os componentes de alerta e feedback 
da plataforma GDAC, usando o contexto de marca acima.

ALERTS / BANNERS (inline)
- Info (azul): ícone info + título + texto + botão "Saiba mais"
- Success (verde): ícone check + texto
- Warning (amarelo): ícone warning + título + texto + botão "Ver detalhes" + X para fechar
- Error (vermelho): ícone error + texto + botão "Tentar novamente"
- Versão compacta (só ícone + texto, sem botão) para cada tipo

TOAST / SNACKBAR
- 4 variações no canto inferior direito: success, error, warning, info
- Com e sem botão de ação
- Com barra de progresso indicando o tempo de fechamento automático

MODAIS
- Modal de confirmação de ação destrutiva: ícone warn, título "Suspender cliente?", 
  texto de consequências, botão "Cancelar" (ghost) + "Confirmar" (error filled)
- Modal de informação: título, corpo com texto e lista, botão "Entendi"
- Modal de bloqueio de CNPJ: ícone de cadeado vermelho, título, texto explicativo, 
  botão "Falar com suporte"

LOADING STATES
- Skeleton loader para card de KPI
- Skeleton loader para linha de tabela (3 linhas)
- Spinner circular sobre botão (botão desabilitado + spinner dentro)
- Overlay de loading em card inteiro

EMPTY STATES
- Sem parceiros cadastrados: ilustração simples, texto, botão CTA
- Sem dados no período: ícone de gráfico vazio, texto explicativo

BADGES E CHIPS DE STATUS
- Todos os status de parceiro e cliente como chips coloridos:
  Prospecto (cinza), Ativo (verde), Inadimplente (amarelo), 
  Suspenso (laranja), Desligado/Cancelado (cinza escuro), Bloqueado (vermelho)

Organize em grid 2 colunas, com labels descritivos. Fundo #F5F7FA.
```

---

## 8. Landing Page do Parceiro — Multi-tenant

```
Crie o layout da landing page customizável de um parceiro fictício "TechSolve Sistemas",
usando o contexto de marca acima como base, mas com personalização do parceiro:

PERSONALIZAÇÃO DO PARCEIRO
- Logo: placeholder "TechSolve" em verde (#2E7D32)
- Cor primária do parceiro: #2E7D32 (verde)
- Slogan: "Soluções de gestão para o seu negócio crescer"

ESTRUTURA
- Mesma estrutura da landing GDAC (navbar, hero, produtos, parceiros, depoimentos, footer)
- Mas com as cores e identidade do parceiro substituindo o primary
- No footer: "Parceiro GDAC" + logo GDAC pequeno + "Powered by GDAC Platform"
- Mostrar que os PRODUTOS exibidos são os que o parceiro selecionou (apenas 2 produtos)
- Hero com foto de fundo (placeholder de cidade/negócio), overlay escuro 60%, texto branco

Mostre como a mesma estrutura absorve a identidade visual do parceiro de forma coerente.
```

---

## Observações para uso no claude.ai/design

- Cada prompt acima é independente — cole um de cada vez
- Sempre cole o **Prompt base** junto com o prompt da tela desejada
- Peça variações: "Mostre a versão dark mode" ou "Mostre a versão mobile"
- Para refinar: "Aumente o espaçamento dos cards", "Deixe o hero mais impactante", etc.
- Salve os HTMLs gerados na pasta `Docs/Platform/Design/Mockups/`

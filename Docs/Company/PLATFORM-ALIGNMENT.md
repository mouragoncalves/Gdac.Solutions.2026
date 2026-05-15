# Alinhamento — Plataforma vs Identidade GDAC

Análise do documento de apresentação (`GDAC-APRESENTACAO.md`) e seus impactos diretos
na plataforma que estamos construindo. Serve como guia de consistência entre a identidade
da empresa e as decisões de produto, conteúdo e design.

---

## 1. O que a GDAC é — e como isso deve aparecer na plataforma

**GDAC = Gestão, Desenvolvimento e Automação Comercial**

Software house brasileira com **perfil híbrido único**: equipe que domina tecnologia
E tem expertise nas áreas administrativa, fiscal e contábil.

**Isso importa para a plataforma porque:**
- O tom da landing page não é "startup de tecnologia" — é **parceiro de negócio**
- Os textos devem reforçar que a GDAC entende o processo do cliente, não só o código
- Os depoimentos e showcases devem refletir resultados de negócio, não apenas técnicos

---

## 2. Portfólio completo — o que cadastrar como Serviços na landing

A landing page exibe serviços cadastrados via painel GDAC. Com base no portfólio real:

| Serviço | Categoria sugerida |
|---------|-------------------|
| Desenvolvimento de sistemas sob medida | Desenvolvimento |
| Automação de processos empresariais | Automação |
| Integração de sistemas | Integração |
| Modernização de sistemas legados | Desenvolvimento |
| Desenvolvimento multiplataforma | Desenvolvimento |
| Customização de aplicações | Desenvolvimento |
| Consultoria tecnológica | Consultoria |
| Implantação e suporte técnico | Suporte |
| Hospedagem gerenciada e VPS | Infraestrutura |
| Assessoria empresarial | Consultoria |
| Certificação digital (e-CPF / e-CNPJ) | Certificação |

> **Ação:** ao cadastrar serviços no `portal-gdac`, usar essas categorias como base.
> Adicionar categoria **Certificação** ao `IntegrationCategory` ou criar enum separado `ServiceCategory`.

---

## 3. Produtos comercializados — o que cadastrar como Produtos

| Produto | Descrição resumida |
|---------|-------------------|
| ControlUP | Controle de estoque + financeiro + DRE, integrado ao ERP do cliente |
| Count+ | Contagem de estoque com identificação de produtos e registro de valores |

> Futuros produtos devem seguir o mesmo padrão de cadastro rico (fotos, vídeos, precificação completa).

---

## 4. Tecnologias — impacto na vitrine de Integrações

A GDAC suporta uma lista ampla de protocolos e plataformas. Para a vitrine de integrações
da landing page, além das categorias já definidas (ERP, E-commerce, Marketplace, etc.),
considerar adicionar:

| Categoria | Justificativa |
|-----------|--------------|
| **Fiscal / NF-e** | SOAP, SPED, eSocial são centrais para o perfil de clientes |
| **Certificação Digital** | Serviço próprio GDAC, destaque na landing |
| **Legado** | Delphi, VB, Pascal — diferencial de modernização de sistemas |

---

## 5. Público-alvo — impacto nos filtros e segmentação

A GDAC atende empresas de diferentes portes e segmentos:

**Por porte:** micro/pequena, média, grande  
**Por segmento:** comércio, serviços, indústria, distribuição, escritórios (contábeis, jurídicos), tecnologia

**Impacto na plataforma:**
- Formulário de cadastro (Onboarding) deve ter campo de **segmento** e **porte**
- Dashboard GDAC pode filtrar parceiros e clientes por segmento
- Landing page pode ter seção "Para quem atendemos" com cards por segmento

---

## 6. Tom e linguagem — guia para textos da plataforma

Com base na identidade GDAC:

| ✅ Usar | ❌ Evitar |
|--------|---------|
| "parceiro de negócio" | "startup" |
| "resultado" e "processo" | Jargão técnico em excesso |
| "soluções sob medida" | "disruptivo", "inovação" sem contexto |
| "comprometimento" e "continuidade" | Promessas vagas |
| Linguagem direta e profissional | Tom informal ou descontraído demais |

---

## 7. Diferenciais que devem aparecer em destaque na landing

Direto do documento "Por Que a GDAC?":

1. Equipe técnica com visão de negócio
2. Stack tecnológico amplo (tecnologia certa para cada desafio)
3. Atuação em sistemas legados E modernos
4. Soluções sob medida
5. Suporte e parceria de longo prazo
6. Responsabilidade única em infraestrutura (desenvolve + hospeda)
7. Assessoria além da tecnologia
8. Portfólio completo (do certificado digital ao sistema sob medida)

> Esses 8 pontos devem embasar a seção "Por Que a GDAC?" da landing page.

---

## 8. Missão / Visão / Valores — uso na plataforma

**Missão:** Oferecer soluções em tecnologia com qualidade, inovação e comprometimento,
contribuindo para a transformação digital das empresas.

**Visão:** Ser reconhecida como referência nacional em desenvolvimento de software,
automação empresarial e integração de sistemas.

**Valores:** Ética · Transparência · Comprometimento · Inovação · Qualidade ·
Responsabilidade · Evolução Contínua · Foco no Cliente · Valorização do Conhecimento Técnico

> Usar na seção "Quem Somos" da landing e no rodapé dos contratos.

---

## 9. Impactos na arquitetura do Content API

Com base no portfólio real, o `ContentService` precisará de uma **categoria**
para classificar os serviços cadastrados. Categorias sugeridas:

```csharp
public enum ServiceCategory
{
    Desenvolvimento     = 1,
    Automacao           = 2,
    Integracao          = 3,
    Consultoria         = 4,
    Suporte             = 5,
    Infraestrutura      = 6,
    Certificacao        = 7,
    Assessoria          = 8
}
```

> **Pendência técnica:** adicionar `ServiceCategory` enum ao `Gdac.Content.Domain`
> e incluir o campo `Category` na entidade `ContentService`.

---

## 10. Segmentos de cliente — campo a adicionar no Onboarding

Para o formulário de cadastro público, adicionar:

```csharp
public enum ClientSegment
{
    Comercio        = 1,   // Comércio varejista e atacadista
    Servicos        = 2,   // Prestação de serviços
    Industria       = 3,
    Distribuidora   = 4,
    Escritorio      = 5,   // Contábil, jurídico, administrativo
    Tecnologia      = 6,
    Outro           = 99
}

public enum CompanySize
{
    Micro    = 1,   // MEI / ME
    Pequena  = 2,   // EPP
    Media    = 3,
    Grande   = 4
}
```

> Esses campos devem ser adicionados ao cadastro de empresa no `Gdac.Core.Api`
> e ao formulário de onboarding.

# INVEST.Web — Investment Tracker (WIP)
[![CI](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci.yml/badge.svg)](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci.yml)
[![Function CI/CD](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci-functions.yml/badge.svg)](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci-functions.yml)

Investment tracker (WIP) built with **ASP.NET Core MVC + EF Core (PostgreSQL)**, applying **Clean Architecture** and DDD-style aggregates (`Acao` / `Tickers`).

This repository is evolved in small iterations to practice:
- architecture and separation of concerns
- cloud integrations on Azure
- asynchronous processing with messaging
- practical backend patterns

Current Azure-focused studies include:
- Azure Functions
- Azure Blob Storage
- Azure Service Bus (queues)
- CI/CD with GitHub Actions

## Demo
<p align="center">
  <img src="docs/demo.gif" width="900" alt="Demo - CRUD de Ação" />
</p>

**Live demo:** [https://mrodriguesweb-invest-bjehhseaagdfcne6.brazilsouth-01.azurewebsites.net/](https://mrodriguesweb-invest-bjehhseaagdfcne6.brazilsouth-01.azurewebsites.net/)

> Nota: ambiente de estudos, pode estar offline/hibernando.

---

## Features

### Core
- CRUD de **Ação** (Create / Edit / Delete).
- **Tickers** como coleção da Ação.
- Regra de domínio: **Name não é editável** após criação.
- Separação entre leitura e escrita (Queries/DTOs vs Commands/Handlers).
- Error handling com middleware/handler global e status pages.

### Company Logos (Azure Functions + Blob)
- Endpoint no MVC: `GET /logos/{empresa}`.
- Azure Function HTTP trigger integrada com Azure Blob Storage.
- Cache em 2 níveis:
  - **Server-side** com `IMemoryCache`, incluindo *negative cache*.
  - **Client-side** com `Cache-Control`.
- Fallback para SVG com iniciais da empresa quando não há logo no blob.

### Quote Update Flow (Azure Service Bus + Azure Functions)
- A aplicação permite disparar atualização de cotações de forma **assíncrona**.
- O MVC atua como **producer**, publicando uma mensagem por ticker em uma **queue** do Azure Service Bus.
- Uma Azure Function com **Service Bus Trigger** atua como **consumer/worker**, processando cada mensagem em background.
- O processamento consulta uma API externa de cotação e persiste um `PriceSnapshot` no banco.
- O fluxo usa conceitos importantes de mensageria:
  - queue-based processing
  - producer / consumer
  - message contract
  - manual message settlement
  - retry com `Abandon`
  - DLQ (*dead-letter queue*) para mensagens inválidas ou falhas persistentes

---

## Messaging Flow

Fluxo simplificado da atualização de cotações:

1. Usuário aciona **Atualizar Cotações** no MVC.
2. O controller chama um handler da camada **Application**.
3. O handler recupera os tickers da ação.
4. Para cada ticker, a aplicação publica uma mensagem JSON em uma queue do **Azure Service Bus**.
5. A **Azure Function** é disparada automaticamente quando há mensagens na queue.
6. A Function desserializa a mensagem e executa a regra de negócio.
7. A cotação é consultada em um provider externo.
8. Um `PriceSnapshot` é persistido no PostgreSQL.
9. Em sucesso, a mensagem é marcada como `Complete`.
10. Em falha transitória, a mensagem pode ser reenfileirada com `Abandon`.
11. Em payload inválido ou falha não recuperável, a mensagem pode ir para a **DLQ**.

This flow was implemented mainly as a learning exercise to understand how asynchronous processing works in practice using Azure Service Bus queues and Azure Functions.

---

## Architecture

Camadas com dependências apontando para dentro:

- **Web (ASP.NET Core MVC)**
  - Controllers, Views e ViewModels
  - inicia fluxos de uso da aplicação
  - expõe o endpoint `/logos/{empresa}`

- **Application**
  - use cases / handlers
  - contratos e abstrações
  - coordena regras de aplicação
  - exemplos:
    - `ICompanyLogoProvider`
    - `IQuoteUpdatePublisher`
    - handlers de atualização de cotações

- **Domain**
  - entidades e invariantes de negócio
  - agregados como `Acao` e `Tickers`

- **Infrastructure**
  - EF Core + PostgreSQL
  - integrações externas
  - implementação de providers
  - publicação no Azure Service Bus
  - persistência de snapshots de cotação

- **Functions**
  - Azure Functions como entrypoint de background processing
  - HTTP trigger para logos
  - Service Bus trigger para processamento assíncrono de mensagens

---

## Azure Resources

Current Azure resources used in this project:

- **Azure App Service** for the MVC application
- **Azure Function App** for background/serverless workloads
- **Azure Storage Account / Blob Storage** for company logos
- **Azure Service Bus** for asynchronous quote update processing

---

## Configuration

### Web App
Required settings (local via User Secrets / Azure App Settings):

- `Azure:CompanyLogo:BaseUrl`
- `Azure:CompanyLogo:FunctionKey`
- `Azure:ServiceBus:ConnectionString`

### Functions
Required settings:

- `SERVICE_BUS`
- `AlphaVantage:ApiKey` *(or other quote provider key, depending on implementation)*
- database connection string used by the Function project

> Secrets are not committed to the repository.

---

## CI/CD

- `ci.yml`
  - build and test da solução

- `ci-functions.yml`
  - build/publish/deploy das Azure Functions

---

## Learning Goals

This project is also a study repository for practicing concepts such as:

- Clean Architecture in a real ASP.NET Core MVC solution
- DDD-style modeling with aggregates
- Azure Functions in different roles (HTTP trigger and Service Bus trigger)
- Azure Blob Storage integration
- asynchronous processing with queues
- retry / DLQ / message lifecycle
- cloud-oriented application design
- CI/CD with GitHub Actions

The goal is not only to build features, but also to understand the architectural trade-offs behind each iteration.

---

## Quickstart (Docker)

Pré-requisitos: Docker Desktop com Docker Compose.

```bash
docker compose up --build
```

---

## Notes

- This is a study project and evolves incrementally.
- Some Azure-hosted resources may be paused, unavailable, or changed over time due to cost control.
- The messaging flow currently uses Azure Service Bus queues. A future evolution may explore topics/subscriptions for publish/subscribe scenarios.

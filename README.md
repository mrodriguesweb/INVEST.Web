# INVEST.Web — Investment Tracker (WIP)
[![CI](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci.yml/badge.svg)](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci.yml)
[![Web App CI/CD](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/feature-azure-app_mrodriguesweb-invest.yml/badge.svg)](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/feature-azure-app_mrodriguesweb-invest.yml)
[![Function CI/CD](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci-functions.yml/badge.svg)](https://github.com/mrodriguesweb/INVEST.Web/actions/workflows/ci-functions.yml)

Investment tracker (WIP) built with **ASP.NET Core MVC + EF Core (PostgreSQL)**, applying Clean Architecture and DDD-style aggregates (Acao/Tickers).  
This repo is evolved in small iterations to practice architecture + cloud integration (AZ-204 focus).

## Demo
<p align="center">
  <img src="docs/demo.gif" width="900" alt="Demo - CRUD de Ação" />
</p>

**Live demo:** https://mrodriguesweb-invest-bjehhseaagdfcne6.brazilsouth-01.azurewebsites.net/
> Nota: ambiente de estudos (pode estar offline/hibernar).

## Features (current)
- CRUD de **Ação** (Create / Edit / Delete).
- **Tickers** como coleção da Ação (normalização via método de domínio).
- Regra: **Name não é editável** após criação.
- Separação Read/Write (Queries DTO vs Commands).
- Error handling com handler global e status pages.

### Company Logos (Azure Functions + Blob)
- Endpoint no MVC: `GET /logos/{empresa}` (usado no `<img src="...">` da tabela).
- Fonte de dados: Azure Function (HTTP) integrada a um container Blob com imagens.
- Cache em 2 níveis:
  - **Server-side**: `IMemoryCache` com cache positivo e *negative cache* (para logos inexistentes).
  - **Client-side**: `Cache-Control: max-age=...` para o browser cachear as imagens.
- Fallback: quando não existe logo no blob, a aplicação retorna um SVG com as iniciais da empresa.

---

## Architecture (Clean Architecture)
Camadas com dependências apontando para dentro:

- **Web (ASP.NET Core MVC)**
  - Controllers + Views + ViewModels.
  - Endpoint `/logos/{empresa}` serve o logo como recurso HTTP e configura caching headers.
- **Application**
  - Use cases/handlers e **abstrações** (ex.: `ICompanyLogoProvider`).
- **Domain**
  - Entidades e invariantes (`Acao`, tickers).
- **Infrastructure**
  - EF Core + PostgreSQL.
  - Integrações externas (ex.: `CompanyLogoFunctionClient`) e decorators (cache).

---

## Configuration
### Web App
Required settings (local via User Secrets / Azure App Settings):
- `Azure:CompanyLogo:BaseUrl`
- `Azure:CompanyLogo:FunctionKey` (secret)

### Azure Resources (overview)
- Azure Function App (HTTP trigger) para servir logos
- Azure Storage Account (Blob container) armazenando `{EMPRESA}.png`. Exemplo: `{BRADESCO}.png`

---

## CI/CD
- `ci.yml`: build/test da solução
- `feature-azure-app_mrodriguesweb-invest.yml`: build/publish/deploy do WebApp na Azure
- `ci-functions.yml`: build/publish/deploy da Azure Function

---

## Quickstart (Docker)
Pré-requisitos: Docker Desktop com Docker Compose.

```bash
docker compose up --build
```

App: http://localhost:8080

# INVEST.Web — Investment Tracker (WIP)

Investment tracker (WIP) built with **ASP.NET Core MVC + EF Core (PostgreSQL)**, applying Clean Architecture, DDD-style aggregates (Acao/Tickers) and Docker Compose(App + Postgres).  
Focus: maintainability, testability, and separation of concerns.

Repository: https://github.com/mrodriguesweb/invesT.Web

> WIP means the project is intentionally incomplete: it is being evolved in small iterations to practice architecture + business rules.

---

## Features (current)
- CRUD de **Ação** (Create / Edit / Delete).
- **Tickers** como coleção da Ação (normalização e sincronização via método de domínio).
- Regra: **Name não é editável** após criação (imutabilidade por design: comando de edit não aceita Name).
- Separação Read/Write:
  - Reads retornam DTOs (queries).
  - Writes carregam agregados via repositório.

---

## Architecture (Clean Architecture)
A aplicação é organizada em camadas, mantendo dependências apontando para dentro.

- **Web (ASP.NET Core MVC)**
  - Controllers + Views + ViewModels.
  - Validação de formato (ModelState / DataAnnotations).
  - Mapeamento ViewModel → Command.
- **Application**
  - Use Cases/Handlers (Create/Edit/Delete) + Commands/Results.
  - Orquestração do fluxo e regras do caso de uso.
  - Depende de abstrações (ex.: IAcaoRepository, ISetorQuery).
- **Domain**
  - Entidades e comportamento (ex.: `Acao.ReplaceTickers(...)`).
  - Invariantes do agregado.
- **Infrastructure**
  - EF Core + PostgreSQL (Npgsql).
  - Implementações de Repositórios/Queries.
  - Migrations.

---

## Tech Stack
- .NET (ASP.NET Core MVC)
- Entity Framework Core
- PostgreSQL
- Npgsql EF Core Provider

---

## Quickstart (Docker)
Pré-requisitos: Docker Desktop (ou Docker Engine) com Docker Compose habilitado.  
​
Subir a aplicação + Postgres.    
Na raiz do repositório, execute:

```
docker compose up --build
```

Esse comando cria e inicia os containers definidos no docker-compose.yml e, com --build, também reconstrói a imagem da aplicação quando houver alterações no Dockerfile/código.

A aplicação ficará disponível em:
http://localhost:8080

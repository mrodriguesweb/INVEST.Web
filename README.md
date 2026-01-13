# INVEST — Investment Tracker (WIP)

Side project para estudar e demonstrar **Clean Architecture** (Uncle Bob) em um contexto real: cadastro e manutenção de ações (com tickers), evoluindo depois para operações, indicadores e regras de negócio de carteira.

> Status: em desenvolvimento (WIP). O objetivo principal do repositório é arquitetura/testabilidade, não “produto final”.

## O que já existe
- CRUD de Ação (Create / Edit / Delete) via ASP.NET Core MVC.
- Tickers como coleção da Ação (sincronização no domínio: dedup + normalização).
- Regra: nome da Ação não é editável após criação.
- Separação em camadas (Web / Application / Domain / Infrastructure).
- Post-Redirect-Get para operações de escrita + mensagens via TempData.

## Arquitetura (visão rápida)
**Web (MVC)**  
- Controllers recebem ViewModels, validam ModelState e mapeiam para Commands.
- Controllers não contêm regra de negócio: apenas adaptação de entrada/saída.

**Application (Use Cases)**  
- Handlers executam casos de uso (Create/Edit/Delete) e retornam Results (sucesso/erros).
- Dependem de abstrações (ex.: `IAcaoRepository`, `ISetorQuery`).

**Domain (Entidades/Regras)**  
- Entidades possuem comportamento (ex.: `Acao.ReplaceTickers(...)`, `Acao.EditarDados(...)`).
- Invariantes/regras ficam protegidas no domínio quando fizer sentido.

**Infrastructure (EF Core)**  
- Implementa repositórios e queries usando EF Core.
- Contém detalhes de persistência (Include, SaveChanges, etc.).

## Tecnologias
- .NET (ASP.NET Core MVC)
- Entity Framework Core
- Banco de dados: PostgreSQL
- Bootstrap (UI)

## Como rodar localmente
### Pré-requisitos
- .NET SDK instalado
- PostgreSQL
- Banco de dados configurado (Ver connection string)

### Passo a passo
1. Clone o repositório:
   ```bash
   git clone https://github.com/mrodriguesweb/INVEST.Web.git
   cd INVEST.Web

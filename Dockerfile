# ============================
# Stage 1: Build (compilação)
# ============================

# Usa a imagem oficial do .NET SDK 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define o diretório de trabalho dentro do container
WORKDIR /app

# Copia os arquivos de projeto (.csproj) de cada camada da sua arquitetura limpa
COPY INVEST.Web/INVEST.Web.csproj INVEST.Web/
COPY INVEST.Infrastructure/INVEST.Infrastructure.csproj INVEST.Infrastructure/
COPY INVEST.Domain/INVEST.Domain.csproj INVEST.Domain/
COPY INVEST.Application/INVEST.Application.csproj INVEST.Application/

# Restaura apontando direto para o Web!
RUN dotnet restore "INVEST.Web/INVEST.Web.csproj"

# Copia todo o restante do código-fonte para dentro do container
COPY . .

# Define o diretório de trabalho como o projeto Web
WORKDIR /app/INVEST.Web

# Publica o projeto em modo Release para a pasta /out
RUN dotnet publish INVEST.Web.csproj -c Release -o /out --no-restore


# ============================
# Stage 2: Runtime (execução)
# ============================

# Usa a imagem oficial do runtime ASP.NET Core 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Define o diretório de trabalho dentro do container
WORKDIR /app

# Copia os artefatos publicados da etapa de build para o runtime
COPY --from=build /out .

# Expõe a porta 8080
EXPOSE 8080

# Define a URL padrão para o ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Define o ponto de entrada do container
ENTRYPOINT ["dotnet", "INVEST.Web.dll"]
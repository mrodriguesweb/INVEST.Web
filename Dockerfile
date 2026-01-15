# ============================
# Stage 1: Build (compilação)
# ============================

# Usa a imagem oficial do .NET SDK 8.0 (tem compilador, ferramentas e tudo necessário para build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define o diretório de trabalho dentro do container (não precisa existir no host, será criado dentro da imagem)
WORKDIR /app

# Copia a solução (.sln) para dentro do container
COPY *.sln ./

# Copia os arquivos de projeto (.csproj) de cada camada da sua arquitetura limpa
COPY INVEST.Web/INVEST.Web.csproj INVEST.Web/
COPY INVEST.Infrastructure/INVEST.Infrastructure.csproj INVEST.Infrastructure/
COPY INVEST.Domain/INVEST.Domain.csproj INVEST.Domain/
COPY INVEST.Application/INVEST.Application.csproj INVEST.Application/

# Restaura os pacotes NuGet (usa os .csproj e .sln copiados)
RUN dotnet restore

# Copia todo o restante do código-fonte para dentro do container
COPY . .

# Define o diretório de trabalho como o projeto Web
WORKDIR /app/INVEST.Web

# Publica o projeto em modo Release para a pasta /out
# --no-restore evita restaurar pacotes novamente (já foi feito antes)
RUN dotnet publish INVEST.Web.csproj -c Release -o /out


# ============================
# Stage 2: Runtime (execução)
# ============================

# Usa a imagem oficial do runtime ASP.NET Core 8.0 (mais leve, sem SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Define o diretório de trabalho dentro do container
WORKDIR /app

# Copia os artefatos publicados da etapa de build para o runtime
COPY --from=build /out .

# Expõe a porta 8080 (informativo para quem usa docker run ou orquestradores)
EXPOSE 8080

# Define a URL padrão para o ASP.NET Core escutar em todas as interfaces na porta 8080
ENV ASPNETCORE_URLS=http://+:8080

# Define o ponto de entrada do container: executa a DLL do seu projeto Web
ENTRYPOINT ["dotnet", "INVEST.Web.dll"]
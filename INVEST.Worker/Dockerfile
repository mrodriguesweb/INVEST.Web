# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia as referências dos projetos
COPY ["INVEST.Worker/INVEST.Worker.csproj", "INVEST.Worker/"]
COPY ["INVEST.Application/INVEST.Application.csproj", "INVEST.Application/"]
COPY ["INVEST.Domain/INVEST.Domain.csproj", "INVEST.Domain/"]
COPY ["INVEST.Infrastructure/INVEST.Infrastructure.csproj", "INVEST.Infrastructure/"]

# Restaura os pacotes
RUN dotnet restore "INVEST.Worker/INVEST.Worker.csproj"

# Copia o resto do código e compila
COPY . .
WORKDIR "/src/INVEST.Worker"
RUN dotnet publish "INVEST.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "INVEST.Worker.dll"]
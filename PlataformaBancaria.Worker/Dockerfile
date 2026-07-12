# Etapa 1: Base leve para rodar aplicações de background
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Etapa 2: Base pesada com o SDK para compilar o código
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todo o código-fonte da solução
COPY . .

# Restaura os pacotes NuGet e compila o projeto do Worker
RUN dotnet restore "PlataformaBancaria.Worker/PlataformaBancaria.Worker.csproj"
WORKDIR "/src/PlataformaBancaria.Worker"
RUN dotnet build "PlataformaBancaria.Worker.csproj" -c Release -o /app/build

# Etapa 3: Publicação dos binários otimizados
FROM build AS publish
RUN dotnet publish "PlataformaBancaria.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 4: Imagem final (copia apenas o que foi compilado na etapa 3)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlataformaBancaria.Worker.dll"]
# Stage 1: Base (Production Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Security: Rodar como usuário não-root (app já existe na imagem dotnet)
USER app
WORKDIR /app
EXPOSE 8080

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ClinicApp.Api/ClinicApp.Api.csproj", "src/ClinicApp.Api/"]
COPY ["src/ClinicApp.Domain/ClinicApp.Domain.csproj", "src/ClinicApp.Domain/"]
COPY ["src/ClinicApp.Application/ClinicApp.Application.csproj", "src/ClinicApp.Application/"]
COPY ["src/ClinicApp.Infrastructure/ClinicApp.Infrastructure.csproj", "src/ClinicApp.Infrastructure/"]
RUN dotnet restore "./src/ClinicApp.Api/ClinicApp.Api.csproj"
COPY . .
WORKDIR "/src/src/ClinicApp.Api"
RUN dotnet build "./ClinicApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ClinicApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final (Clean Production Image)
FROM base AS final
WORKDIR /app
# Security: Copiar arquivos já com dono 'app'
COPY --from=publish --chown=app:app /app/publish .

# Preparar o entrypoint como root para garantir permissões de execução
USER root
COPY src/ClinicApp.Api/entrypoint.sh /app/entrypoint.sh
# Garantir final de linha LF (Linux) e permissão de execução
RUN sed -i 's/\r$//' /app/entrypoint.sh && chmod +x /app/entrypoint.sh

# Voltar para o usuário 'app'
USER app

ENTRYPOINT ["/app/entrypoint.sh"]
# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos SOLO el código fuente (NO la solution)
COPY src/ ./src/

# Restauramos ÚNICAMENTE el proyecto Web
RUN dotnet restore src/AutonomousMarketingPlatform.Web/AutonomousMarketingPlatform.Web.csproj

# Publicamos el proyecto Web
RUN dotnet publish src/AutonomousMarketingPlatform.Web/AutonomousMarketingPlatform.Web.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos SOLO el resultado publicado
COPY --from=build /app/publish .

# Render define el puerto dinámicamente
# La variable PORT se inyecta en runtime, no en build time
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "AutonomousMarketingPlatform.Web.dll"]


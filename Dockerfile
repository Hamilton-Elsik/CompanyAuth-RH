# =========================
# STAGE 1: Build & Publish
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los .csproj primero para aprovechar la cache de Docker
COPY ["CompanyAuth.Api/CompanyAuth.Api.csproj", "CompanyAuth.Api/"]
COPY ["CompanyAuth.Infrastructure/CompanyAuth.Infrastructure.csproj", "CompanyAuth.Infrastructure/"]
COPY ["CompanyAuth.Application/CompanyAuth.Application.csproj", "CompanyAuth.Application/"]
COPY ["CompanyAuth.Domain/CompanyAuth.Domain.csproj", "CompanyAuth.Domain/"]

# Restaurar dependencias
RUN dotnet restore "CompanyAuth.Api/CompanyAuth.Api.csproj"

# Copiar todo el código
COPY . .

# Compilar y publicar en modo Release
WORKDIR "/src/CompanyAuth.Api"
RUN dotnet publish "CompanyAuth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# STAGE 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Google Cloud Run usa el puerto 8080 por defecto
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copiamos lo publicado desde el stage de build
COPY --from=build /app/publish .

# Exponemos el puerto (para desarrollo/local; Cloud Run lo ignora pero es útil)
EXPOSE 8080

# Ejecutar la API
ENTRYPOINT ["dotnet", "CompanyAuth.Api.dll"]

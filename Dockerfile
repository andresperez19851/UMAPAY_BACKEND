# Imagen base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["UmaPay.Api/UmaPay.Api.csproj", "UmaPay.Api/"]
COPY ["UmaPay.Domain/UmaPay.Domain.csproj", "UmaPay.Domain/"]
COPY ["UmaPay.Resource/UmaPay.Resource.csproj", "UmaPay.Resource/"]
COPY ["UmaPay.Interface/UmaPay.Interface.csproj", "UmaPay.Interface/"]
COPY ["UmaPay.IoC/UmaPay.IoC.csproj", "UmaPay.IoC/"]
COPY ["UmaPay.Integration.Nuvei/UmaPay.Integration.Nuvei.csproj", "UmaPay.Integration.Nuvei/"]
COPY ["UmaPay.Integration.Wompi/UmaPay.Integration.Wompi.csproj", "UmaPay.Integration.Wompi/"]
COPY ["UmaPay.Middleware.Sap/UmaPay.Middleware.Sap.csproj", "UmaPay.Middleware.Sap/"]
COPY ["UmaPay.Repository/UmaPay.Repository.csproj", "UmaPay.Repository/"]
COPY ["UmaPay.Service/UmaPay.Service.csproj", "UmaPay.Service/"]
COPY ["UmaPay.Shared/UmaPay.Shared.csproj", "UmaPay.Shared/"]


RUN dotnet restore "./UmaPay.Api/UmaPay.Api.csproj"

# Copiar el resto del código fuente
COPY . .
WORKDIR "/src/UmaPay.Api"


# Construir la aplicación
RUN dotnet build "./UmaPay.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UmaPay.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
USER app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UmaPay.Api.dll"]
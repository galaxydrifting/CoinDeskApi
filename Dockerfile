# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CoinDeskApi.Api/CoinDeskApi.Api.csproj", "CoinDeskApi.Api/"]
COPY ["CoinDeskApi.Infrastructure/CoinDeskApi.Infrastructure.csproj", "CoinDeskApi.Infrastructure/"]
COPY ["CoinDeskApi.Core/CoinDeskApi.Core.csproj", "CoinDeskApi.Core/"]
RUN dotnet restore "./CoinDeskApi.Api/CoinDeskApi.Api.csproj"
COPY . .
WORKDIR "/src/CoinDeskApi.Api"
RUN dotnet build "./CoinDeskApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CoinDeskApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoinDeskApi.Api.dll"]

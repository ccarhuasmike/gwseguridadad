# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Security.slnx ./
COPY src/Security.Domain/Security.Domain.csproj src/Security.Domain/
COPY src/Security.Application/Security.Application.csproj src/Security.Application/
COPY src/Security.Infrastructure/Security.Infrastructure.csproj src/Security.Infrastructure/
COPY src/Security.Api/Security.Api.csproj src/Security.Api/
COPY tests/Security.Tests/Security.Tests.csproj tests/Security.Tests/
RUN dotnet restore src/Security.Api/Security.Api.csproj

COPY . .
RUN dotnet publish src/Security.Api/Security.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Security.Api.dll"]

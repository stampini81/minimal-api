# Dockerfile para Minimal API

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["mininal-api.csproj", "."]
RUN dotnet restore "mininal-api.csproj"
COPY . .
RUN dotnet build "mininal-api.csproj" -c Release -o /app/build
RUN dotnet publish "mininal-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "mininal-api.dll"]

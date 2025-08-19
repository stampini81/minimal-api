# Minimal API — Desafio DIO

Projeto de ASP.NET Core Minimal API (.NET 8) com autenticação JWT, CRUDs, validações, Swagger, testes automatizados e deploy com Docker + MySQL.

## Sumário
- Funcionalidades
- Como executar (local e Docker)
- Endpoints (principais rotas)
- Exemplos de requisições
- Testes automatizados
- Ferramentas e principais comandos
- Deploy (Dockerfile e docker-compose)
- Links úteis

## Funcionalidades
- Autenticação JWT (Bearer) com proteção por perfis: Adm e Editor
- CRUD de Administradores e Veículos
- Validações de dados e respostas com erros de validação
- Filtro e paginação no GET de veículos (`pagina`, `nome`, `marca`)
- Endpoint de Healthcheck: `GET /health`
- Swagger com suporte a bearer token
- Seed inicial (Administrador e Veículos) e migrações do EF Core

## Como executar

Pré-requisitos:
- .NET SDK 8 instalado
- MySQL (local) ou Docker (para subir o banco e a API com compose)

### 1) Execução local (sem Docker)
1. Configure a conexão em `Api/appsettings.json` (ConnectionStrings:MySql) e uma chave JWT forte em `Jwt`.
2. Aplique as migrações no banco:
   ```powershell
   dotnet ef database update --project Api
   ```
3. Rode a API:
   ```powershell
   dotnet run --project Api
   ```
4. Acesse o Swagger (verifique a porta informada no console). Com Docker Compose é 5004; local pode variar conforme seu ambiente/launchSettings.

Variáveis de ambiente úteis (sobrepõem appsettings.json):
- `ConnectionStrings__MySql`: string de conexão do MySQL
- `Jwt`: chave de assinatura JWT (mín. 32 bytes)
- `ASPNETCORE_ENVIRONMENT`: por exemplo, `Development`

### 2) Execução via Docker Compose
1. Com Docker Desktop ativo, na raiz do repositório:
   ```powershell
   docker compose up -d --build
   ```
2. A API ficará disponível em `http://localhost:5004/swagger` após o MySQL ficar saudável.
3. Para parar e remover os recursos:
   ```powershell
   docker compose down -v
   ```

O arquivo `docker-compose.yml` já define:
- MySQL 8 com volume persistente
- API .NET 8 publicada e exposta em `5004:80`
- Variáveis de ambiente para conexão e JWT

## Endpoints

### Home e Saúde
- `GET /` — Informação básica
- `GET /health` — Healthcheck (anônimo)

### Administradores
- `POST /administradores/login` — Login e geração de token (retorna token JWT)
- `GET /administradores` — Lista admins (Role: Adm)
- `GET /administradores/{id}` — Detalhe por ID (Role: Adm)
- `POST /administradores` — Cria admin (Role: Adm)

### Veículos
- `POST /veiculos` — Cria (Roles: Adm, Editor)
- `GET /veiculos` — Lista (autenticado). Suporta filtros: `?pagina=1&nome=gol&marca=vw`
- `GET /veiculos/{id}` — Detalhe (Roles: Adm, Editor)
- `PUT /veiculos/{id}` — Atualiza (Role: Adm)
- `DELETE /veiculos/{id}` — Remove (Role: Adm)

No Swagger, clique em “Authorize”, informe `Bearer {seu_token}` (sem aspas) e autorize.

## Exemplos de requisições

### Login
```http
POST /administradores/login
Content-Type: application/json
{
  "email": "administrador@teste.com",
  "senha": "123456"
}
```

### Criar veículo
```http
POST /veiculos
Authorization: Bearer {token}
Content-Type: application/json
{
  "nome": "Fusca",
  "marca": "Volkswagen",
  "ano": 1980
}
```

### Listar veículos paginando e filtrando
```http
GET /veiculos?pagina=1&nome=g&marca=volks
Authorization: Bearer {token}
```

## Testes automatizados

Para executar:
```powershell
dotnet test
```

Notas:
- Testes de unidade usam `EF Core InMemory`, evitando dependência de MySQL
- Testes de request usam `Microsoft.AspNetCore.Mvc.Testing`
- Inclui teste de saúde (`/health`) e autenticação de login

## Ferramentas e principais comandos

Ferramentas e libs:
- ASP.NET Core 8 (Minimal APIs)
- Entity Framework Core 8 + Pomelo MySQL
- Autenticação JWT Bearer
- Swagger (Swashbuckle)
- MSTest + WebApplicationFactory + EF Core InMemory
- Dockerfile + Docker Compose

Comandos úteis (PowerShell):
- Restaurar/compilar: `dotnet restore; dotnet build`
- Rodar local: `dotnet run --project Api`
- Aplicar migrações: `dotnet ef database update --project Api`
- Rodar testes: `dotnet test`
- Subir com Docker: `docker compose up -d --build`
- Derrubar Docker: `docker compose down -v`

## Deploy

Dockerfile (na pasta `Api/`):
```dockerfile
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
```

docker-compose (na raiz do repositório):
```yaml
services:
  db:
    image: mysql:8.0
    container_name: minimal_mysql
    environment:
      - MYSQL_ROOT_PASSWORD=root
      - MYSQL_DATABASE=minimal_api
    ports:
      - "3306:3306"
    volumes:
      - dbdata:/var/lib/mysql
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      timeout: 5s
      retries: 5
  api:
    build:
      context: ./Api
      dockerfile: Dockerfile
    container_name: minimal_api
    depends_on:
      db:
        condition: service_healthy
    environment:
      ASPNETCORE_URLS: "http://+:80"
      ConnectionStrings__MySql: "Server=db;Database=minimal_api;Uid=root;Pwd=root;"
      Jwt: "minimal-api-super-secret-key-32bytes-minimum-2025"
      ASPNETCORE_ENVIRONMENT: "Development"
    ports:
      - "5004:80"
volumes:
  dbdata:
```

## Links úteis
- Documentação ASP.NET Minimal APIs: https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis
- Swagger: https://swagger.io/

—

Projeto criado para estudo e portfólio, alinhado ao desafio DIO de Minimal API.

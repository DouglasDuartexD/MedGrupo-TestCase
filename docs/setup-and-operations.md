# Setup And Operations

## Pre-requisitos

- `Docker Desktop`
- `.NET 9 SDK`

## Arquivos de configuracao

Arquivos relevantes:

- `.env`
- `.env.example`
- `docker-compose.yml`
- `src/Contacts.API/appsettings.json`

## Variaveis de ambiente

Exemplo:

```env
ASPNETCORE_ENVIRONMENT=Development
API_PORT=5299
SQLSERVER_PORT=14330
SQLSERVER_DB=ContactsDb
SQLSERVER_USER=sa
SA_PASSWORD=YourStrong@Pass123
SQLSERVER_EDITION=Developer
```

## Subindo com Docker Compose

Na raiz do projeto:

```bash
docker compose up -d --build
```

Verificacao:

```bash
docker compose ps
```

Logs:

```bash
docker compose logs api
docker compose logs sqlserver
```

URLs esperadas:

- API: `http://localhost:5299`
- Swagger: `http://localhost:5299/swagger`
- SQL Server: `localhost,14330`

## Encerrando o ambiente

Parar containers:

```bash
docker compose down
```

Resetar completamente o ambiente, incluindo o banco:

```bash
docker compose down -v
```

Use `down -v` apenas quando quiser recriar o banco do zero.

## Observação importante sobre senha do SQL Server

Se a senha do `sa` for alterada no `.env` depois que o volume do banco ja existir, o SQL Server pode continuar usando a senha antiga armazenada no volume.

Sintoma comum:

- o container do `sqlserver` fica `unhealthy`
- os logs mostram `Login failed for user 'sa'`

Correção recomendada para ambiente local:

```bash
docker compose down -v
docker compose up -d --build
```

## Rodando sem Docker Compose

Se voce quiser usar um SQL Server ja existente:

1. Ajuste a connection string em `src/Contacts.API/appsettings.json`
2. Rode as migrations
3. Inicie a API manualmente

Comandos:

```bash
dotnet ef database update --project src/Contacts.Infrastructure/Contacts.Infrastructure.csproj --startup-project src/Contacts.API/Contacts.API.csproj
dotnet run --project src/Contacts.API/Contacts.API.csproj
```

## Migrations

O projeto aplica migrations automáticamente no startup da API quando:

- o ambiente não é `Testing`
- o provider configurado é relacional

Tambem é possivel aplicar migrations manualmente com `dotnet ef`.

## Observabilidade minima

Comandos uteis:

```bash
docker compose ps
docker compose logs -f
dotnet test Contacts.slnx -c Release
```

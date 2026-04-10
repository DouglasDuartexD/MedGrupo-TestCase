# Contacts API

API REST para gerenciamento de contatos, desenvolvida com:

- `.NET 9`
- `ASP.NET Core Web API`
- `Entity Framework Core`
- `SQL Server`
- `FluentValidation`
- `xUnit`

## Objetivo

O projeto expõe uma API para cadastro e manutenção de contatos, com regras de negocio, validação de entrada e persistencia em `SQL Server`.

Fluxo principal de uso em desenvolvimento:

1. Ajustar o arquivo `.env`
2. Subir banco e API com `docker compose`
3. Abrir o Swagger
4. Executar os testes quando necessario

## Documentação

Documentação complementar:

- [docs/README.md](./docs/README.md)
- [docs/architecture.md](./docs/architecture.md)
- [docs/api-reference.md](./docs/api-reference.md)
- [docs/setup-and-operations.md](./docs/setup-and-operations.md)
- [docs/testing-and-quality.md](./docs/testing-and-quality.md)

## Pre-requisitos

- `Docker Desktop`
- `.NET 9 SDK`

## Inicio Rápido

### 1. Ir para a raiz do projeto

```bash
cd d:/medgroupo-test
```

### 2. Revisar o arquivo `.env`

O projeto usa váriaveis de ambiente locais para configurar portas, banco e senha do SQL Server.

Arquivo local:

- [`.env`](./.env)

Arquivo modelo:

- [`.env.example`](./.env.example)

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

Observações:

- o arquivo `.env` e local e esta no `.gitignore`
- se você alterar a senha do `sa` em um ambiente que já tem volume criado, pode ser necessário recriar o volume do banco

### 3. Subir a aplicacao

```bash
docker compose up -d --build
```

### 4. Verificar se os serviços subiram

```bash
docker compose ps
```

Você deve ver os serviços `api` e `sqlserver` como `Up`.

### 5. Abrir a aplicação

- API: `http://localhost:5299`
- Swagger: `http://localhost:5299/swagger`
- SQL Server no host: `localhost,14330`

## Comandos Uteis

Subir sem rebuild:

```bash
docker compose up -d
```

Parar os containers:

```bash
docker compose down
```

Ver status:

```bash
docker compose ps
```

Ver logs da API:

```bash
docker compose logs api
```

Ver logs do banco:

```bash
docker compose logs sqlserver
```

Seguir logs em tempo real:

```bash
docker compose logs -f
```

Resetar completamente o ambiente, incluindo o volume do banco:

```bash
docker compose down -v
```

Use `down -v` apenas quando você quiser recriar o banco do zero.

## Banco de Dados

Configuração padrão no ambiente Docker:

- Host: `localhost,14330`
- Database: valor de `SQLSERVER_DB`
- User: valor de `SQLSERVER_USER`
- Password: valor de `SA_PASSWORD`

Exemplo de connection string para ferramentas externas:

```text
Server=localhost,14330;Database=ContactsDb;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True;Encrypt=False;
```

## Rodando Sem Docker Compose

Se voce quiser usar um `SQL Server` ja existente:

### 1. Ajustar a connection string

Edite [appsettings.json](d:/medgroupo-test/src/Contacts.API/appsettings.json).

Exemplo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ContactsDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. Aplicar as migrations

```bash
dotnet ef database update --project src/Contacts.Infrastructure/Contacts.Infrastructure.csproj --startup-project src/Contacts.API/Contacts.API.csproj
```

### 3. Subir a API

```bash
dotnet run --project src/Contacts.API/Contacts.API.csproj
```

## Endpoints

Base path: `/api/contacts`

| Metodo | Rota | Descricao |
|---|---|---|
| `POST` | `/api/contacts` | Cria um contato |
| `GET` | `/api/contacts` | Lista contatos ativos |
| `GET` | `/api/contacts/{id}` | Busca um contato ativo |
| `PUT` | `/api/contacts/{id}` | Atualiza um contato ativo |
| `PATCH` | `/api/contacts/{id}/deactivate` | Desativa um contato |
| `PATCH` | `/api/contacts/{id}/activate` | Ativa um contato |
| `DELETE` | `/api/contacts/{id}` | Remove um contato |

Payload de exemplo:

```json
{
  "name": "Maria Silva",
  "birthDate": "1990-05-10",
  "sex": "Female"
}
```

Valores aceitos para `sex`:

- `Female`
- `Male`
- `Other`
- `NotInformed`

## Regras de Negócio

- o contato deve ter `18 anos ou mais`
- `birthDate` não pode ser futura
- idade `0` não e permitida
- `age` e cálculada em tempo de execução
- listagem e detalhe retornam apenas contatos ativos
- `PUT` opera apenas em contatos ativos
- desativação e lógica com `isActive = false`
- exclusão com `DELETE` e fisica

## Validação

As validações de entrada usam `FluentValidation`.

Principais regras:

- `name` é obrigatório
- `name` não pode ser vazio
- `name` tem máximo de `150` caracteres
- `birthDate` é obrigatória
- `birthDate` não pode ser `0001-01-01`
- `birthDate` não pode ser futura
- idade deve ser maior ou igual a `18`
- `sex` é obrigatório
- `sex` deve estar dentro do enum suportado

Formato padrao de erro:

```json
{
  "type": "validation_error",
  "errors": [
    "Name is required."
  ]
}
```

## Testes

Executar todos os testes:

```bash
dotnet test Contacts.slnx -c Release
```

Cobertura atual:

- testes de dominio
- testes de use cases
- testes de validators
- testes de integracao HTTP

Estado atual:

- `50 testes`
- `0 falhas`

## Estrutura da Solução

```text
Contacts.slnx
|-- docker-compose.yml
|-- .env
|-- docs
|-- src
|   |-- Contacts.API
|   |-- Contacts.Application
|   |-- Contacts.Domain
|   `-- Contacts.Infrastructure
`-- tests
    `-- Contacts.Tests
```

## Arquitetura adotada

- `Clean Architecture` simplificada
- monolito modular por projetos
- principios `SOLID`

## Observações

- a solução principal do projeto e [Contacts.slnx](d:/medgroupo-test/Contacts.slnx)
- o banco sobe junto com a API via `docker compose`
- as migrations são aplicadas automaticamente no startup da aplicação

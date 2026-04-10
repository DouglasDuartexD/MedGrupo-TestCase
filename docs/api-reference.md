# API Reference

## Base URL

Em ambiente local com Docker:

- `http://localhost:5299`

Base path dos endpoints:

- `/api/contacts`

## Modelo de dados

### ContactRequest

```json
{
  "name": "Maria Silva",
  "birthDate": "1990-05-10",
  "sex": "Female"
}
```

Campos:

- `name`: string obrigatória, maximo de 150 caracteres
- `birthDate`: data obrigatória no formato `yyyy-MM-dd`
- `sex`: enum obrigatório

Valores aceitos para `sex`:

- `Female`
- `Male`
- `Other`
- `NotInformed`

Observação:

- valores numéricos para `sex` sao rejeitados

### ContactResponse

```json
{
  "id": "d6ed2d4d-b0e5-46b3-80e4-76b0ecb55bf4",
  "name": "Maria Silva",
  "birthDate": "1990-05-10",
  "sex": "Female",
  "age": 35,
  "isActive": true,
  "createdAt": "2026-04-10T18:30:00Z",
  "updatedAt": "2026-04-10T18:30:00Z"
}
```

## Endpoints

### POST /api/contacts

Criar um novo contato.

Respostas:

- `201 Created`: contato criado com sucesso
- `400 Bad Request`: payload invalido

### GET /api/contacts

Listar todos os contatos ativos.

Respostas:

- `200 OK`

### GET /api/contacts/{id}

Retornar os detalhes de um contato ativo.

Respostas:

- `200 OK`
- `404 Not Found`: contato nao encontrado ou inativo

### PUT /api/contacts/{id}

Atualizar um contato ativo.

Respostas:

- `200 OK`
- `400 Bad Request`
- `404 Not Found`

### PATCH /api/contacts/{id}/deactivate

Desativar logicamente um contato.

Respostas:

- `200 OK`
- `404 Not Found`
- `409 Conflict`: operacao invalida para o estado atual

### PATCH /api/contacts/{id}/activate

Reativar um contato inativo.

Respostas:

- `200 OK`
- `404 Not Found`
- `409 Conflict`: operacao invalida para o estado atual

### DELETE /api/contacts/{id}

Remover fisicamente um contato.

Respostas:

- `204 No Content`
- `404 Not Found`

## Regras de negocio

- o contato deve ter 18 anos ou mais
- `birthDate` não pode ser futura
- idade zero não e permitida
- `age` é calculada em tempo de execucao
- listagem e detalhe retornam apenas contatos ativos
- atualizacao opera apenas em contatos ativos
- desativacao é logica com `isActive = false`
- exclusão com `DELETE` é fisica

## Erros padronizados

### Erro de validacao

```json
{
  "type": "validation_error",
  "errors": [
    "Name is required."
  ]
}
```

### Erro de conflito

```json
{
  "type": "conflict_error",
  "errors": [
    "Contact is already inactive."
  ]
}
```

### Erro interno

```json
{
  "type": "internal_error",
  "errors": [
    "An unexpected error occurred."
  ]
}
```

## Observacoes sobre Swagger

- o Swagger está disponivel apenas em ambiente `Development`
- o schema OpenAPI foi ajustado para refletir corretamente campos obrigatórios e nao anulaveis

# Architecture

## Visao geral

O projeto segue uma `Clean Architecture` simplificada, organizada como um monolito modular em projetos separados.

Objetivos principais:

- isolar regras de negocio do framework web
- reduzir acoplamento entre camadas
- facilitar testes unitarios e de integração
- manter a solução simples para avaliação tecnica

## Estrutura da solução

```text
src
|-- Contacts.API
|-- Contacts.Application
|-- Contacts.Domain
`-- Contacts.Infrastructure

tests
`-- Contacts.Tests
```

## Responsabilidade por camada

### Contacts.API

Responsável por:

- receber requisicoes HTTP
- expor os endpoints REST
- configurar serializacao JSON
- publicar Swagger
- centralizar tratamento de erros via middleware

Principais pontos:

- rejeita valores numéricos para enums no JSON
- aplica migrations automaticamente no startup quando o provider e relacional
- usa um `SchemaFilter` para deixar o contrato do Swagger coerente com a API real

### Contacts.Application

Responsável por:

- orquestrar os casos de uso
- definir DTOs de entrada e saida
- aplicar validacao com `FluentValidation`
- depender de abstracoes em vez de implementacoes concretas

Casos de uso atuais:

- criar contato
- listar contatos ativos
- buscar contato ativo por id
- atualizar contato ativo
- desativar contato
- ativar contato
- excluir contato

### Contacts.Domain

Responsável por:

- representar a entidade `Contact`
- aplicar regras centrais de negócio
- calcular idade em tempo de execução
- impedir estados invalidos via exceçôes de dominio

Regras importantes no dominio:

- contato deve ter 18 anos ou mais
- `BirthDate` não pode ser maior que a data de referência
- idade zero não é permitida
- `Age` não é persistida

### Contacts.Infrastructure

Responsável por:

- persistencia com `Entity Framework Core`
- configuração do `DbContext`
- mapeamento da entidade `Contact`
- implementação do repositorio
- abstração de data e hora via `IDateTimeProvider`

Banco adotado:

- `SQL Server`

### Contacts.Tests

Responsável por:

- testes de dominio
- testes de validacao
- testes de integracao HTTP

## Principios adotados

- `S` de `SOLID`: cada classe tem responsabilidade clara
- `O`: comportamento estendido com novos casos de uso e serviços sem reescrever o núcleo
- `L`: contratos simples entre camadas
- `I`: dependencias pequenas e focadas
- `D`: camadas superiores dependem de abstrações, nao de implementações concretas

## Fluxo de requisicao

1. O controller recebe a requisição HTTP
2. O request é desserializado
3. O caso de uso executa a validação com `FluentValidation`
4. O dominio aplica as regras centrais
5. O repositorio persiste ou consulta dados no banco
6. A resposta é convertida para `ContactResponse`
7. O middleware transforma exceções conhecidas em respostas HTTP padronizadas

## Decisoes tecnicas relevantes

- `Age` e calculada sob demanda a partir de `BirthDate`
- contatos sao desativados logicamente com `IsActive`
- exclusão com `DELETE` é fisica
- listagem e detalhe consideram apenas contatos ativos
- a referência de data usada nas regras vem de `IDateTimeProvider`

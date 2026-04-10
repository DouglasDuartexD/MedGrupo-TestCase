# Testing And Quality

## Estrategia de testes

O projeto combina testes de dominio, validação e integração HTTP para cobrir as principais regras funcionais e tecnicas.

Categorias atuais:

- testes de dominio
- testes de validators
- testes de integração HTTP

## Escopo coberto

### Dominio

Valida:

- calculo de idade
- bloqueio de menor de idade
- bloqueio de data futura
- comportamento de ativação e desativação

### Validação

Valida:

- obrigatoriedade de `name`
- obrigatoriedade de `birthDate`
- obrigatoriedade de `sex`
- tamanho máximo de `name`
- datas invalidas
- enum invalido

### Integração HTTP

Valida:

- retorno `201` para criação valida
- retorno `400` para payloads invalidos
- formato padronizado de erro
- rejeição de enums numericos
- comportamento de `PUT` com payload invalido

## Execução

Rodar todos os testes:

```bash
dotnet test Contacts.slnx -c Release
```

## Estado atual

- `50 testes`
- `0 falhas`

## Praticas de qualidade adotadas

- separação de responsabilidades por camada
- validação com `FluentValidation`
- regras centrais protegidas no dominio
- middleware para padronização de erros
- Swagger alinhado ao contrato real da API

## Pontos de extensao futuros

- testes de repositório com banco real em ambiente isolado
- testes de contrato OpenAPI
- pipeline CI com build, testes e validação de formatação

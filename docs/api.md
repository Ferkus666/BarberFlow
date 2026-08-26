# API REST

A API do BarberFlow está versionada sob `/api/v1` e é documentada pelo Swagger/OpenAPI.

## Autenticação

`POST /api/v1/auth/login` retorna um JWT. Endpoints protegidos usam as roles Cliente, Profissional e Administrador conforme a operação.

## Recursos principais

- `/api/v1/auth`
- `/api/v1/servicos`
- `/api/v1/profissionais`
- `/api/v1/clientes`
- `/api/v1/reservas`

## Convenções

- DTOs são usados como contratos de entrada e saída.
- Listagens suportam paginação e filtros quando aplicável.
- Exceções não tratadas passam pelo middleware global.
- Operações relevantes geram logs estruturados.
- A documentação interativa fica disponível em `/swagger` no ambiente de desenvolvimento/demonstração.

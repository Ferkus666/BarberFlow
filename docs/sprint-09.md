# Sprint 9 — API REST

A Sprint 9 adiciona uma API REST versionada em `/api/v1`, documentada com Swagger e protegida por JWT nas operações de escrita e consulta administrativa.

## Recursos

- `POST /api/v1/auth/login`
- `GET/POST/PUT/DELETE /api/v1/servicos`
- `GET /api/v1/profissionais`
- `GET/POST/PUT /api/v1/clientes`
- `GET/POST/PATCH/DELETE /api/v1/reservas`
- paginação com `page` e `pageSize`
- filtros por nome, status, período, profissional e atividade
- DTOs e AutoMapper
- respostas padronizadas
- Swagger em `/swagger`

## Teste rápido

1. Execute o projeto.
2. Abra `/swagger`.
3. Use `POST /api/v1/auth/login` com o administrador de desenvolvimento.
4. Copie o `accessToken`.
5. Clique em **Authorize** e informe apenas o token.
6. Teste os endpoints protegidos.

Esta sprint não altera o banco de dados.

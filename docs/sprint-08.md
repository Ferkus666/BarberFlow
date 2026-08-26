# Sprint 8 — Arquitetura profissional e qualidade

## Objetivo

Fortalecer a base técnica do BarberFlow antes da criação da API REST, reduzindo validações dispersas e preparando contratos reutilizáveis.

## Implementado

- FluentValidation integrado ao pipeline MVC.
- Validadores para registro, profissional, horário, serviço e nova reserva.
- AutoMapper e perfil de mapeamento centralizado.
- DTOs de serviço, profissional e reserva.
- Middleware global de exceções com `ProblemDetails` para futuras rotas `/api`.
- Página amigável de erro para o MVC, com `traceId` para diagnóstico.
- Logging estruturado em autenticação, reservas e dashboard.
- `OperationResult` para padronizar resultados de serviços nas próximas refatorações.
- Extensões de configuração para reduzir responsabilidades do `Program.cs`.

## Decisões

A estrutura continua em um único projeto para não criar complexidade artificial em um portfólio. As pastas `Application`, `Middleware` e `Extensions` separam responsabilidades e permitem uma migração futura para múltiplos projetos, caso necessário.

Os atributos de DataAnnotations foram mantidos por compatibilidade com as telas atuais. O FluentValidation passa a concentrar regras mais expressivas e será priorizado nas próximas funcionalidades.

## Banco de dados

A sprint não altera tabelas ou colunas. Nenhuma migration nova é necessária.

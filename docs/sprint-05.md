# Sprint 5 — Gestão completa de reservas

## Objetivo
Completar o ciclo de atendimento para cliente, profissional e administrador.

## Funcionalidades
- Cliente: histórico, cancelamento e reagendamento.
- Profissional: agenda diária e atualização do status do atendimento.
- Administrador: consulta global e filtros de reservas.
- Conta de acesso do profissional criada pelo administrador.

## Conceitos praticados
- Autorização por roles.
- Relacionamento entre domínio e ASP.NET Core Identity.
- Consultas LINQ com filtros opcionais.
- Regras de transição de status.
- ViewModels específicos para cada fluxo.

## Banco de dados
Esta sprint adiciona `UsuarioId` em `Profissionais`. Gere uma migration antes de executar.

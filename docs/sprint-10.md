# Sprint 10 — Testes automatizados e confiabilidade

## Objetivo

Adicionar uma suíte automatizada para as regras críticas do BarberFlow e preparar a solução para evolução com menor risco de regressão.

## Projeto de testes

Foi adicionado `tests/BarberFlow.Tests`, referenciado pela solução principal, com:

- xUnit;
- FluentAssertions;
- Moq;
- Entity Framework Core InMemory;
- coverlet para cobertura.

## Cenários cobertos

- data passada não gera horários;
- duração, jornada e conflitos são respeitados;
- reserva cancelada libera o horário;
- indicadores e faturamentos do dashboard;
- validação de serviços e intervalos de 15 minutos;
- configuração e mapeamento do AutoMapper;
- retornos padronizados com `OperationResult`.

## Execução

```powershell
dotnet restore
dotnet test BarberFlow.sln
```

Com cobertura:

```powershell
dotnet test BarberFlow.sln --collect:"XPlat Code Coverage"
```

## Observação

O ambiente usado para preparar a sprint não possui o SDK do .NET. A suíte foi revisada estruturalmente, mas a execução final deve ser realizada em uma máquina com .NET 8 instalado.

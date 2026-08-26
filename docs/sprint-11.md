# Sprint 11 — UX, profissionais e avaliações

## Objetivo
Aproximar o BarberFlow de um produto real e melhorar sua apresentação no portfólio sem aumentar desnecessariamente a complexidade da arquitetura.

## Entregas
- Perfil público de profissionais.
- Upload e substituição de foto com nome aleatório e allowlist de extensões.
- Especialidade, anos de experiência e Instagram.
- Exibição pública de serviços e agenda semanal.
- Avaliação de atendimento concluído com nota de 1 a 5 e comentário opcional.
- Restrição de uma avaliação por reserva.
- Média e histórico recente de avaliações no perfil do profissional.
- Refinamento visual e responsivo.

## Alteração de banco
Esta sprint adiciona campos em `Profissionais` e a tabela `Avaliacoes`. Gere e aplique uma migration antes de testar as telas:

```powershell
dotnet ef migrations add Sprint11ProfissionaisAvaliacoes
dotnet ef database update
```

## Como testar
1. Entre como administrador e edite um profissional.
2. Preencha especialidade, experiência, Instagram e envie uma foto JPG/PNG/WEBP de até 3 MB.
3. Abra a página pública `Profissionais`.
4. Conclua uma reserva na agenda do profissional.
5. Entre com o cliente dono da reserva e abra `Minhas reservas`.
6. Clique em `Avaliar atendimento`, envie uma nota e confirme se ela aparece no perfil público.


> A partir da v2.1, a migration de consolidação da Sprint 11 já acompanha o projeto. Não crie outra migration com esse nome.

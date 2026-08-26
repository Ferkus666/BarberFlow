# Modelo de dados

## Relacionamentos principais

```mermaid
erDiagram
    CLIENTE ||--o{ RESERVA : realiza
    PROFISSIONAL ||--o{ RESERVA : atende
    SERVICO ||--o{ RESERVA : corresponde
    PROFISSIONAL ||--o{ HORARIO_TRABALHO : possui
    PROFISSIONAL ||--o{ PROFISSIONAL_SERVICO : oferece
    SERVICO ||--o{ PROFISSIONAL_SERVICO : associado
    RESERVA ||--o| AVALIACAO : recebe
    PROFISSIONAL ||--o{ AVALIACAO : recebe
    CLIENTE ||--o{ AVALIACAO : publica
```

## Entidades

- **Cliente:** dados do cliente vinculados ao fluxo de reservas.
- **Profissional:** perfil público, vínculo de usuário, especialidade e situação ativa.
- **Servico:** nome, descrição, duração, preço e situação ativa.
- **HorarioTrabalho:** períodos de trabalho por profissional e dia da semana.
- **Reserva:** cliente, profissional, serviço, início, fim e status.
- **Avaliacao:** nota/comentário associado a um atendimento concluído.
- **ProfissionalServico:** relacionamento muitos-para-muitos entre profissional e serviço.

As migrations do Entity Framework Core versionam as mudanças estruturais do banco.

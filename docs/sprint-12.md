# Sprint 12 — Dados de demonstração e Docker

## Objetivo

Tornar o BarberFlow simples de demonstrar e reproduzir em qualquer máquina, sem exigir cadastro manual dos profissionais a cada novo banco.

## Dados de demonstração

Quando `SeedDemoData:Enabled` estiver habilitado, o sistema cria quatro profissionais de demonstração de forma idempotente:

- Rafael Martins — Fade e cortes modernos
- Lucas Almeida — Barba e acabamento
- Matheus Rocha — Corte clássico
- Diego Santos — Corte e barba

Todos ficam ativos, possuem serviços associados e jornada de segunda a sábado, das 09:00 às 18:00. Também são criadas contas Identity com role `Profissional`, usando a senha local de demonstração `Demo123`.

O seed pode ser executado várias vezes: ele completa dados ausentes sem duplicar profissionais, vínculos de serviço ou horários.

## Docker

A solução inclui `Dockerfile`, `docker-compose.yml`, `.dockerignore` e `.env.example`.

```bash
docker compose up --build
```

A aplicação fica disponível em `http://localhost:8080` e o Swagger em `http://localhost:8080/swagger`.

O SQL Server é executado em outro container e mantém os dados em um volume Docker.

## Migrations

Foi adicionada uma migration idempotente de consolidação dos campos introduzidos na Sprint 11. Ela evita recriar `UsuarioId` quando a coluna já existir em um banco local antigo.

No Docker, `Database:ApplyMigrationsOnStartup=true` aplica automaticamente as migrations antes dos seeds. Localmente o comportamento continua desabilitado por padrão, preservando o fluxo normal com `dotnet ef database update`.

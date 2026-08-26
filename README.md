# BarberFlow

Sistema de reservas para barbearias desenvolvido em **C# / .NET 8** como projeto de portfólio. O sistema permite gerenciar serviços, profissionais, jornadas e reservas, com áreas específicas para cliente, profissional e administrador.

## Principais recursos

- Agendamento com cálculo de disponibilidade e prevenção de conflitos.
- ASP.NET Core Identity com perfis Cliente, Profissional e Administrador.
- API REST versionada (`/api/v1`) com autenticação JWT e Swagger.
- Gestão de profissionais, serviços, jornadas, reservas e avaliações.
- Dashboard e relatórios administrativos.
- Perfis públicos e upload validado de fotos dos profissionais.
- Testes automatizados com xUnit e FluentAssertions.
- Docker Compose com aplicação e SQL Server.
- Integração contínua com GitHub Actions.

## Tecnologias

**.NET 8 · ASP.NET Core MVC · Web API · Entity Framework Core · SQL Server · Identity · JWT · Swagger/OpenAPI · AutoMapper · FluentValidation · xUnit · Bootstrap · Chart.js · Docker · GitHub Actions**

## Executar localmente

Pré-requisitos: **.NET SDK 8** e **SQL Server LocalDB/SQL Server**.

```powershell
dotnet restore
dotnet ef database update
dotnet test BarberFlow.sln
dotnet run
```

Também é possível abrir `BarberFlow.sln` no Visual Studio e executar com **F5** após atualizar o banco.

### Contas de demonstração

- Administrador: `admin@barberflow.local` / `Admin123`
- Profissionais: `rafael@barberflow.local`, `lucas@barberflow.local`, `matheus@barberflow.local`, `diego@barberflow.local`
- Senha dos profissionais: `Demo123`

As credenciais acima existem somente para demonstração local.

## Docker

```bash
docker compose up --build
```

Aplicação: `http://localhost:8080`  
Swagger: `http://localhost:8080/swagger`

## Testes e CI

```powershell
dotnet test BarberFlow.sln
```

O workflow em `.github/workflows/ci.yml` executa automaticamente **restore, build, testes com cobertura e validação Docker** em pushes e pull requests.

## Documentação

Documentação complementar sobre arquitetura, banco e API está disponível em `docs/`. O histórico técnico do projeto está em `CHANGELOG.md`.

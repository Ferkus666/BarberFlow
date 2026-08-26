# BarberFlow — Histórico de mudanças

## Portfolio Release — Sprint 15
- Revisão final da interface e remoção de textos internos de desenvolvimento.
- README reduzido ao conteúdo essencial para avaliação do projeto.
- Remoção de arquivo local do Visual Studio (`.csproj.user`).
- Revisão da estrutura para publicação no GitHub.
- Mantidos testes automatizados, Docker, API REST e pipeline de CI como critérios da release.
- Nenhuma nova funcionalidade ou alteração de banco nesta release.

## v2.3 — Sprint 14: GitHub e portfólio

- Interface pública sem rótulos internos de sprint.
- Home ajustada para comunicação de produto.
- README profissional com stack, arquitetura, execução, API, testes, Docker e CI.
- `.gitignore` revisado para evitar build artifacts, arquivos locais e segredos.
- Documentação de arquitetura, banco de dados, API e roteiro de screenshots.
- Diagramas Mermaid adicionados à documentação.
- Estrutura preparada para a release final e publicação no GitHub.


## v2.2 — Sprint 13: integração contínua
- GitHub Actions executando restore, build e testes a cada push ou pull request.
- Build da solução validado em configuração Release.
- Cobertura de testes coletada com XPlat Code Coverage.
- Resultados de testes e cobertura publicados como artifact do workflow.
- Validação automática do `docker-compose.yml`.
- Build do `Dockerfile` condicionado à aprovação dos testes.
- Concorrência configurada para cancelar execuções antigas da mesma branch.
- Dependabot semanal para pacotes NuGet e GitHub Actions.
- Documentação da Sprint 13 adicionada.

## v2.1 — Sprint 12: dados de demonstração e Docker
- Seed idempotente com quatro profissionais prontos para demonstração.
- Jornadas padrão de segunda a sábado, das 09:00 às 18:00.
- Serviços associados automaticamente por profissional.
- Contas Identity de demonstração com role `Profissional`.
- Dockerfile multi-stage para .NET 8.
- Docker Compose com aplicação + SQL Server 2022 e volume persistente.
- Aplicação automática das migrations no ambiente Docker.
- `.env.example` e `.dockerignore`.
- Migration idempotente para consolidar `UsuarioId`, campos de perfil e avaliações sem duplicar colunas existentes.
- ModelSnapshot atualizado para refletir o modelo atual.

## v2.0 — Sprint 11: UX, profissionais e avaliações
- Upload seguro de foto para profissionais (JPG/PNG/WEBP, até 3 MB).
- Novos campos de apresentação: especialidade, anos de experiência e Instagram.
- Página pública de profissionais e perfil detalhado com serviços e agenda semanal.
- Avaliações de 1 a 5 estrelas vinculadas a atendimentos concluídos.
- Um atendimento só pode receber uma avaliação.
- Cards, estados visuais e responsividade refinados.
- Navegação pública ganhou acesso à equipe.

# Correção v1.9.1

- Adicionado `GlobalUsings.cs` ao projeto `BarberFlow.Tests`.
- Importação global do namespace `Xunit`, corrigindo os erros CS0246 relacionados a `[Fact]`, `[Theory]` e `[InlineData]`.

# Changelog
## v1.7 — Sprint 8

- FluentValidation integrado ao MVC.
- AutoMapper e DTOs adicionados.
- Middleware global de exceções.
- Página de erro com traceId.
- Logging estruturado em fluxos críticos.
- Resultado padronizado para serviços.
- Organização de configuração por extensões.


## v1.6 — Sprint 7
- Dashboard administrativo com indicadores reais.
- Gráficos com Chart.js.
- Relatórios avançados e filtros.
- DashboardService e ViewModels específicos.
- Resumo e destaque de agenda para profissionais.
- Toasts, confirmações e melhorias de UX.
- Nenhuma alteração de banco necessária.

## v1.5 — Sprint 6
- Fluxo completo de agendamento e painel administrativo inicial.

## v1.8 — Sprint 9
- API REST versionada em `/api/v1`.
- Swagger/OpenAPI com autenticação Bearer.
- Autenticação JWT para a API.
- Endpoints de serviços, profissionais, clientes e reservas.
- Paginação, filtros e respostas padronizadas.
- DTOs de entrada e saída sem exposição direta das entidades.
- AutoMapper atualizado para versão sem o alerta anterior da 13.0.1.

## v1.9 — Sprint 10

- Adicionado projeto `BarberFlow.Tests` à solução.
- Criados testes com xUnit, FluentAssertions e EF Core InMemory.
- Cobertura das regras de disponibilidade, conflitos e reservas canceladas.
- Testes dos indicadores financeiros do dashboard.
- Testes do FluentValidation, AutoMapper e `OperationResult`.
- Adicionado coverlet para geração de cobertura.
- Corrigida expressão LINQ incompatível com árvores de expressão no `DashboardService`.
- Adicionado `partial Program` para facilitar futuros testes de integração da API.

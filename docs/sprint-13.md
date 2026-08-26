# Sprint 13 — Integração Contínua com GitHub Actions

## Objetivo

Automatizar a validação técnica do BarberFlow a cada `push` e `pull request`, garantindo que uma alteração só seja considerada saudável quando a solução restaura dependências, compila, passa pelos testes automatizados e continua gerando a imagem Docker.

## Pipeline

O workflow está em `.github/workflows/ci.yml` e executa dois jobs.

### 1. Restore, build and test

1. Faz checkout do repositório.
2. Instala o .NET 8.
3. Executa `dotnet restore BarberFlow.sln`.
4. Compila em `Release` com `dotnet build`.
5. Executa toda a suíte com `dotnet test`.
6. Coleta cobertura no formato gerado pelo Coverlet.
7. Publica resultados de testes e cobertura como artifact do GitHub Actions por 14 dias.

Qualquer falha de compilação ou teste deixa o pipeline vermelho.

### 2. Validate Docker image

Esse job só inicia se o job de qualidade for aprovado.

1. Valida o arquivo `docker-compose.yml` com `docker compose config`.
2. Executa um build real do `Dockerfile`.

Isso reduz o risco de o projeto continuar funcionando localmente enquanto a configuração Docker fica quebrada.

## Concorrência

O workflow cancela uma execução antiga quando um novo commit é enviado para a mesma branch. Isso evita gastar tempo de CI validando commits que já foram substituídos.

## Dependabot

Também foi adicionado `.github/dependabot.yml` para verificar semanalmente atualizações de:

- pacotes NuGet;
- actions utilizadas pelo GitHub Actions.

As atualizações são propostas por Pull Request; nada é atualizado automaticamente sem revisão.

## Cobertura de testes

A Sprint 13 coleta cobertura, mas não bloqueia o pipeline por percentual mínimo. Nesta fase do portfólio, o objetivo é tornar a métrica visível sem criar uma meta artificial que incentive testes apenas para aumentar um número.

## Badge

Depois que o projeto estiver no GitHub, o badge do workflow pode ser colocado no README usando o usuário e o nome reais do repositório. Essa etapa será feita junto com a publicação do projeto para evitar um link quebrado ou fictício.

## Como explicar em uma entrevista

O BarberFlow utiliza integração contínua para executar automaticamente restore, build, testes e validação da imagem Docker a cada alteração enviada ao GitHub. O pipeline impede que erros básicos de compilação ou regressões cobertas pelos testes passem despercebidos e deixa os resultados de testes/cobertura disponíveis como artifacts da execução.

# Portfolio Release

Esta release encerra o ciclo funcional do BarberFlow. Antes de publicar uma alteração no repositório, valide:

1. `dotnet restore`
2. `dotnet build BarberFlow.sln --configuration Release`
3. `dotnet test BarberFlow.sln`
4. `docker compose config`
5. execução local dos fluxos principais de cliente, profissional e administrador

O histórico de sprints permanece apenas como documentação técnica; a interface pública não exibe números de sprint ou versão.

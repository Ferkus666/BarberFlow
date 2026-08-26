using BarberFlow.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Migrations;

[DbContext(typeof(BarberFlowDbContext))]
[Migration("20260826165000_Sprint11PortfolioProfiles")]
public partial class Sprint11PortfolioProfiles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Migration idempotente para manter compatibilidade com bancos que já receberam
        // UsuarioId e os campos da Sprint 11 por uma migration local anterior.
        migrationBuilder.Sql("""
IF COL_LENGTH('Profissionais', 'UsuarioId') IS NULL
    ALTER TABLE [Profissionais] ADD [UsuarioId] nvarchar(450) NULL;
IF COL_LENGTH('Profissionais', 'Especialidade') IS NULL
    ALTER TABLE [Profissionais] ADD [Especialidade] nvarchar(120) NULL;
IF COL_LENGTH('Profissionais', 'AnosExperiencia') IS NULL
    ALTER TABLE [Profissionais] ADD [AnosExperiencia] int NULL;
IF COL_LENGTH('Profissionais', 'Instagram') IS NULL
    ALTER TABLE [Profissionais] ADD [Instagram] nvarchar(120) NULL;
IF COL_LENGTH('Profissionais', 'FotoUrl') IS NULL
    ALTER TABLE [Profissionais] ADD [FotoUrl] nvarchar(300) NULL;

IF OBJECT_ID(N'[Avaliacoes]', N'U') IS NULL
BEGIN
    CREATE TABLE [Avaliacoes] (
        [Id] int NOT NULL IDENTITY,
        [Nota] int NOT NULL,
        [Comentario] nvarchar(500) NULL,
        [DataCriacao] datetime2 NOT NULL,
        [ClienteId] int NOT NULL,
        [ProfissionalId] int NOT NULL,
        [ReservaId] int NOT NULL,
        CONSTRAINT [PK_Avaliacoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Avaliacoes_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avaliacoes_Profissionais_ProfissionalId] FOREIGN KEY ([ProfissionalId]) REFERENCES [Profissionais] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avaliacoes_Reservas_ReservaId] FOREIGN KEY ([ReservaId]) REFERENCES [Reservas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Profissionais_UsuarioId' AND object_id = OBJECT_ID('Profissionais'))
    CREATE UNIQUE INDEX [IX_Profissionais_UsuarioId] ON [Profissionais] ([UsuarioId]) WHERE [UsuarioId] IS NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Profissionais_AspNetUsers_UsuarioId')
    ALTER TABLE [Profissionais] ADD CONSTRAINT [FK_Profissionais_AspNetUsers_UsuarioId]
        FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Avaliacoes_ClienteId' AND object_id = OBJECT_ID('Avaliacoes'))
    CREATE INDEX [IX_Avaliacoes_ClienteId] ON [Avaliacoes] ([ClienteId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Avaliacoes_ProfissionalId' AND object_id = OBJECT_ID('Avaliacoes'))
    CREATE INDEX [IX_Avaliacoes_ProfissionalId] ON [Avaliacoes] ([ProfissionalId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Avaliacoes_ReservaId' AND object_id = OBJECT_ID('Avaliacoes'))
    CREATE UNIQUE INDEX [IX_Avaliacoes_ReservaId] ON [Avaliacoes] ([ReservaId]);
""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
IF OBJECT_ID(N'[Avaliacoes]', N'U') IS NOT NULL DROP TABLE [Avaliacoes];
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Profissionais_AspNetUsers_UsuarioId')
    ALTER TABLE [Profissionais] DROP CONSTRAINT [FK_Profissionais_AspNetUsers_UsuarioId];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Profissionais_UsuarioId' AND object_id = OBJECT_ID('Profissionais'))
    DROP INDEX [IX_Profissionais_UsuarioId] ON [Profissionais];
IF COL_LENGTH('Profissionais', 'FotoUrl') IS NOT NULL ALTER TABLE [Profissionais] DROP COLUMN [FotoUrl];
IF COL_LENGTH('Profissionais', 'Instagram') IS NOT NULL ALTER TABLE [Profissionais] DROP COLUMN [Instagram];
IF COL_LENGTH('Profissionais', 'AnosExperiencia') IS NOT NULL ALTER TABLE [Profissionais] DROP COLUMN [AnosExperiencia];
IF COL_LENGTH('Profissionais', 'Especialidade') IS NOT NULL ALTER TABLE [Profissionais] DROP COLUMN [Especialidade];
IF COL_LENGTH('Profissionais', 'UsuarioId') IS NOT NULL ALTER TABLE [Profissionais] DROP COLUMN [UsuarioId];
""");
    }
}

using BarberFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Tests.Helpers;

internal static class TestDbContextFactory
{
    public static BarberFlowDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<BarberFlowDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        return new BarberFlowDbContext(options);
    }
}

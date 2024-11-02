using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Database;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.ConfigureWarnings(
            warningsConfigurationBuilderAction: t =>
            {
                t.Log(CoreEventId.SensitiveDataLoggingEnabledWarning);
                t.Throw();
            });

        optionsBuilder.UseSqlServer(
            connectionString: "Data Source=localhost,1433;Initial Catalog=TestDb;Persist Security Info=True;User ID=SA;Password=123qwerty!;Encrypt=True;Trust Server Certificate=True",
            options => options.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Constants.DatabaseInfastructureSchema));

        return new(optionsBuilder.Options);
    }
}
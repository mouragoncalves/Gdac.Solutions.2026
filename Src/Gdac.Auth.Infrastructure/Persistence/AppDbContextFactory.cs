using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gdac.Auth.Infrastructure.Persistence;

/// <summary>
/// Usado apenas pelo dotnet-ef em design time (migrations). Não afeta runtime.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(
                "Server=localhost;Database=gdac_auth_design;Uid=root;Pwd=design;",
                ServerVersion.Parse("11.4.0-mariadb"),
                o => o.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .Options;

        return new AppDbContext(options);
    }
}

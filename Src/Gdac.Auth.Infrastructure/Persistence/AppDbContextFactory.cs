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
        // Usa o socket do XAMPP local para geração de migrations.
        // Em CI o socket não existe — o dotnet-ef não precisa de conexão real para gerar migrations.
        var connectionString = "Server=/Applications/XAMPP/xamppfiles/var/mysql/mysql.sock;" +
                               "Database=gdac_auth_dev;Uid=root;Pwd=;CharSet=utf8mb4;SslMode=None;";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 4, 0)),
                o => o.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .Options;

        return new AppDbContext(options);
    }
}

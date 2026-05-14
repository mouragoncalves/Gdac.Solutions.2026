using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gdac.Core.Infrastructure.Persistence;

public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=/Applications/XAMPP/xamppfiles/var/mysql/mysql.sock;" +
                               "Database=gdac_core_dev;Uid=root;Pwd=;CharSet=utf8mb4;SslMode=None;";

        var options = new DbContextOptionsBuilder<CoreDbContext>()
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 4, 0)),
                o => o.MigrationsAssembly(typeof(CoreDbContext).Assembly.FullName))
            .Options;

        return new CoreDbContext(options);
    }
}

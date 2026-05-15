using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gdac.Content.Infrastructure.Persistence;

public class ContentDbContextFactory : IDesignTimeDbContextFactory<ContentDbContext>
{
    public ContentDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=/Applications/XAMPP/xamppfiles/var/mysql/mysql.sock;" +
                               "Database=gdac_content_dev;Uid=root;Pwd=;CharSet=utf8mb4;SslMode=None;";

        var options = new DbContextOptionsBuilder<ContentDbContext>()
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 4, 0)),
                o => o.MigrationsAssembly(typeof(ContentDbContext).Assembly.FullName))
            .Options;

        return new ContentDbContext(options);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gdac.Onboarding.Infrastructure.Persistence;

public class OnboardingDbContextFactory : IDesignTimeDbContextFactory<OnboardingDbContext>
{
    public OnboardingDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=/Applications/XAMPP/xamppfiles/var/mysql/mysql.sock;" +
                               "Database=gdac_onboarding_dev;Uid=root;Pwd=;CharSet=utf8mb4;SslMode=None;";

        var options = new DbContextOptionsBuilder<OnboardingDbContext>()
            .UseMySql(
                connectionString,
                new MariaDbServerVersion(new Version(10, 4, 0)),
                o => o.MigrationsAssembly(typeof(OnboardingDbContext).Assembly.FullName))
            .Options;

        return new OnboardingDbContext(options);
    }
}

using Gdac.Onboarding.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Onboarding.Infrastructure.Persistence;

public class OnboardingDbContext(DbContextOptions<OnboardingDbContext> options) : DbContext(options)
{
    public DbSet<Registration>           Registrations           => Set<Registration>();
    public DbSet<LeadDistributionConfig> LeadDistributionConfigs => Set<LeadDistributionConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnboardingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

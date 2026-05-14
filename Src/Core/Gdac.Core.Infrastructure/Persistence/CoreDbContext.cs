using Gdac.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Core.Infrastructure.Persistence;

public class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<UserProfile>     UserProfiles     => Set<UserProfile>();
    public DbSet<Company>         Companies        => Set<Company>();
    public DbSet<UserCompanyLink> UserCompanyLinks => Set<UserCompanyLink>();
    public DbSet<CompanyMember>   CompanyMembers   => Set<CompanyMember>();
    public DbSet<CompanyOffice>   CompanyOffices   => Set<CompanyOffice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

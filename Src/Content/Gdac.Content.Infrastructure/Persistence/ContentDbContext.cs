using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Content.Infrastructure.Persistence;

public class ContentDbContext(DbContextOptions<ContentDbContext> options) : DbContext(options)
{
    public DbSet<Banner>              Banners          => Set<Banner>();
    public DbSet<Testimonial>         Testimonials     => Set<Testimonial>();
    public DbSet<Integration>         Integrations     => Set<Integration>();
    public DbSet<ShowcaseItem>        ShowcaseItems    => Set<ShowcaseItem>();
    public DbSet<Product>             Products         => Set<Product>();
    public DbSet<ProductMedia>        ProductMedia     => Set<ProductMedia>();
    public DbSet<ProductPriceHistory> ProductPriceHistory => Set<ProductPriceHistory>();
    public DbSet<ContentService>      Services         => Set<ContentService>();
    public DbSet<ServiceMedia>        ServiceMedia     => Set<ServiceMedia>();
    public DbSet<ServicePriceHistory> ServicePriceHistory => Set<ServicePriceHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

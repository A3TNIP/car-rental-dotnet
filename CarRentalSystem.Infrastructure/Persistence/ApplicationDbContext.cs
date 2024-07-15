using CarRentalSystem.Application.Common;
using CarRentalSystem.Domain.Base;
using CarRentalSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = CarRentalSystem.Domain.Entities.File;

namespace CarRentalSystem.Infrastructure.Persistence;

public class ApplicationDbContext: IdentityDbContext<User, IdentityRole, string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Config> Configs { get; set; }
    public DbSet<Rent> Rents { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Damage> Damages { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure eager loading for all entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var navigationProperties = entityType.GetNavigations();
            
            foreach (var navigationProperty in navigationProperties)
            {
                navigationProperty.SetIsEagerLoaded(true);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseEntity> entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    break;
            }
        }
        
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}
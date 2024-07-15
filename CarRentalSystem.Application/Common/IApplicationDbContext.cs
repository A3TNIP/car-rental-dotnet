using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using File = CarRentalSystem.Domain.Entities.File;

namespace CarRentalSystem.Application.Common;

public interface IApplicationDbContext
{
    DbSet<Car> Cars { get; set; }
    DbSet<File> Files { get; set; }
    DbSet<Document> Documents { get; set; }
    DbSet<Config> Configs { get; set; }
    DbSet<Rent> Rents { get; set; }
    DbSet<Offer> Offers { get; set; }
    DbSet<Damage> Damages { get; set; }
    DbSet<Bill> Bills { get; set; }
    DbSet<Payment> Payments { get; set; }
}
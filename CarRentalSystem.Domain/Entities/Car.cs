using System.ComponentModel.DataAnnotations.Schema;
using CarRentalSystem.Domain.Base;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;

public class Car: BaseEntity
{
    public string Model { get; set; }
    public string Make { get; set; }
    public string Color { get; set; }
    public string LicensePlate { get; set; }
    public DateOnly BuildYear { get; set; }
    public string Brand { get; set; }
    public Decimal Rate { get; set; }
    public string? ImageUrl { get; set; }
    
    public CarStatus Status { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;
using CarRentalSystem.Domain.Base;

namespace CarRentalSystem.Domain.Entities;

public class Damage: BaseEntity
{
    [ForeignKey("Rent")]
    public Guid RentalId { get; set; }
    public virtual Rent? Rent { get; set; }
    
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string DamagedParts { get; set; }

    [ForeignKey("Employee")]
    public string? EmployeeId { get; set; }
    public virtual User? Employee { get; set; }
    
    public decimal? RepairCost { get; set; }
}
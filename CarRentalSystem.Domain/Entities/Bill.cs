using System.ComponentModel.DataAnnotations.Schema;
using CarRentalSystem.Domain.Base;

namespace CarRentalSystem.Domain.Entities;

public class Bill: BaseEntity
{
    [ForeignKey("Rent")]
    public Guid RentalId { get; set; }
    public virtual Rent? Rent { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal RepairCost { get; set; }
    public decimal Rate { get; set; }
    public decimal Discount { get; set; }
    public int days { get; set; }
}
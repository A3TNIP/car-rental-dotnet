using System.ComponentModel.DataAnnotations.Schema;
using CarRentalSystem.Domain.Base;

namespace CarRentalSystem.Domain.Entities;

public class Payment: BaseEntity
{
    [ForeignKey("Bill")]
    public Guid BillId { get; set; }
    public virtual Bill? Bill { get; set; }
    public decimal PaidAmount { get; set; }
    public string? PaymentMethod { get; set; }
    [ForeignKey("Customer")]
    public string CustomerId { get; set; }
    [ForeignKey("Employee")]
    public string? EmployeeId { get; set; }
    public virtual User? Customer { get; set; }
    public virtual User? Employee { get; set; }
}
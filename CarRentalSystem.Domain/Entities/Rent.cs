using System.ComponentModel.DataAnnotations.Schema;
using CarRentalSystem.Domain.Base;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;

public class Rent : BaseEntity
{
    [ForeignKey("RequestedBy")]
    public string RequestedById { get; set; }
    public User? RequestedBy { get; set; }
    [ForeignKey("Car")]
    public Guid CarId { get; set; }
    public Car Car { get; set; }
    [ForeignKey("ApprovedBy")]
    public string? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    [ForeignKey("Offer")]
    public Guid? OfferId { get; set; }
    public Offer? Offer { get; set; }
    public RentalStatus Status { get; set; }
}
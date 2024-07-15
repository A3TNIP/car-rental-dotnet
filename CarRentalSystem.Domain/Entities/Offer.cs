using CarRentalSystem.Domain.Base;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;

public class Offer: BaseEntity
{
    public string OfferName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Discount { get; set; }
    public OfferType Type { get; set; }
}
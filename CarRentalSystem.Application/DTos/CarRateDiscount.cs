namespace CarRentalSystem.Application.DTos;

public class CarRateDiscount
{
    public Guid Id { get; set; }
    public decimal Rate { get; set; }
    public decimal DiscountedRate { get; set; }
}
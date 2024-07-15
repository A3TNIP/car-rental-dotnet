namespace CarRentalSystem.Application.DTos;

public class SalesDto
{
    public string carId { get; set; }
    public string carName { get; set; }
    public int count { get; set; }
    public decimal rate { get; set; }
    public decimal total { get; set; }
}
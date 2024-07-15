namespace CarRentalSystem.Application.DTos;

public class SalesFilterDto
{
    public string? UserName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AuthorizedBy { get; set; }
}
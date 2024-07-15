namespace CarRentalSystem.Application.DTos;

public class KhaltiConfirmationResponseDto
{
    public string token { get; set; }
    public string mobile { get; set; }
    public int amount { get; set; }
    public string fee_amount { get; set; }
    public string product_identity { get; set; }
    public string product_name { get; set; }
}
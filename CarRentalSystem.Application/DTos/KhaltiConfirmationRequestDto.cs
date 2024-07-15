using Newtonsoft.Json;

namespace CarRentalSystem.Application.DTos;

public class KhaltiConfirmationRequestDto
{
    [JsonProperty("public_key")]
    public string? PublicKey { get; set; }
    [JsonProperty("token")]
    public string Token { get; set; }
    [JsonProperty("confirmation_code")]
    public string ConfirmationCode { get; set; }
    [JsonProperty("transaction_pin")]
    public string TransactionPin { get; set; }
}
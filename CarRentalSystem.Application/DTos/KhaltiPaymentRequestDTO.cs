using Newtonsoft.Json;

namespace CarRentalSystem.Application.DTos;

public class KhaltiPaymentRequestDTO
{
    [JsonProperty("public_key")]
    public string? PublicKey { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }
}

public class KhaltiInitiateRequest
{
    [JsonProperty("public_key")]
    public string? PublicKey { get; set; }

    [JsonProperty("mobile")]
    public string Mobile { get; set; }

    [JsonProperty("transaction_pin")]
    public string TransactionPin { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("product_identity")]
    public string ProductIdentity { get; set; }

    [JsonProperty("product_name")]
    public string ProductName { get; set; }

    [JsonProperty("product_url")]
    public string? ProductUrl { get; set; }
}
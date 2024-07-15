using CarRentalSystem.Domain.Entities;
using Newtonsoft.Json;

namespace CarRentalSystem.Application.DTos;

public class KhaltiPaymentResponse
{
    [JsonProperty("idx")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public PaymentType Type { get; set; }

    [JsonProperty("state")]
    public PaymentState State { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("fee_amount")]
    public int FeeAmount { get; set; }

    [JsonProperty("refunded")]
    public bool Refunded { get; set; }

    [JsonProperty("created_on")]
    public DateTime CreatedOn { get; set; }

    [JsonProperty("ebanker")]
    public object Ebanker { get; set; }

    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("merchant")]
    public Merchant Merchant { get; set; }
}

public class PaymentType
{
    [JsonProperty("idx")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class PaymentState
{
    [JsonProperty("idx")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("template")]
    public string Template { get; set; }
}

public class Merchant
{
    [JsonProperty("idx")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("mobile")]
    public string Mobile { get; set; }
}

public class KhaltiInitiateResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}
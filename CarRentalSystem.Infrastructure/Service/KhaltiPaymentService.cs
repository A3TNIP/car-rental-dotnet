using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CarRentalSystem.Infrastructure.Service;

public class KhaltiPaymentService : IKhaltiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public KhaltiPaymentService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }


    public async Task<BaseResponseDto<KhaltiInitiateResponse>> InitiateTransaction(KhaltiInitiateRequest dto)
    {
        // Set url
        var url = $"{_configuration["Khalti:ApiUrl"]}initiate/";

        // set payload
        var requestData = new KhaltiInitiateRequest()
        {
            PublicKey = GetPublicKey(),
            Mobile = dto.Mobile,
            TransactionPin = dto.TransactionPin,
            Amount = dto.Amount * 100,
            ProductIdentity = dto.ProductIdentity,
            ProductName = dto.ProductName,
            ProductUrl = ""
        };

        // set content
        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Make request
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new DomainException($"Failed to initiate Khalti payment.", statusCode: 400);
        }

        // Get response content
        var responseContent = await response.Content.ReadAsStringAsync();

        // Return response
        return new BaseResponseDto<KhaltiInitiateResponse>(
            JsonConvert.DeserializeObject<KhaltiInitiateResponse>(responseContent)!);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paymentRequest"></param>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    public async Task<BaseResponseDto<KhaltiPaymentResponse>> ConfirmTransaction(
        KhaltiConfirmationRequestDto paymentRequest)
    {
        // Get public key
        paymentRequest.PublicKey = GetPublicKey();

        // Set url
        var url = $"{_configuration["Khalti:ApiUrl"]}confirm/";

        // Set content
        var content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");

        // Make request
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new DomainException($"Failed to confirm Khalti payment.", statusCode: 400);
        }

        // Get response content
        var deserializeObject = await response.Content.ReadFromJsonAsync<KhaltiConfirmationResponseDto>();

        // Verify payment
        var reqDto = new KhaltiPaymentRequestDTO()
        {
            Amount = deserializeObject!.amount,
            Token = deserializeObject!.token
        };

        // Verify payment
        var verification = await VerifyPayment(reqDto);

        // Return response
        return new BaseResponseDto<KhaltiPaymentResponse>(verification.Data);
    }

    /// <summary>
    /// Verifies whether the payment was successful or not
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    public async Task<BaseResponseDto<KhaltiPaymentResponse>> VerifyPayment(KhaltiPaymentRequestDTO dto)
    {
        // Set url
        var url = $"{_configuration["Khalti:ApiUrl"]}verify/";

        // Set payload
        var payload = new KhaltiPaymentRequestDTO()
        {
            PublicKey = GetPublicKey(),
            Amount = dto.Amount,
            Token = dto.Token
        };

        // Initiate http client
        using var httpClient = new HttpClient();
        
        // Set authorization header
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", GetSecretKey());

        // Set content
        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        // Make request
        var response = await httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new DomainException($"Failed to verify Khalti payment.", statusCode: 400);
        }

        // Get response content
        var responseContent = await response.Content.ReadAsStringAsync();

        // Return response
        return new BaseResponseDto<KhaltiPaymentResponse>(
            JsonConvert.DeserializeObject<KhaltiPaymentResponse>(responseContent)!);
    }

    /// <summary>
    /// Returns the public key from appsettings.json
    /// </summary>
    /// <returns></returns>
    private string GetPublicKey()
    {
        return _configuration["Khalti:PublicKey"];
    }

    /// <summary>
    /// Returns the secret key from appsettings.json
    /// </summary>
    /// <returns></returns>
    private string GetSecretKey()
    {
        return _configuration["Khalti:SecretKey"];
    }
}
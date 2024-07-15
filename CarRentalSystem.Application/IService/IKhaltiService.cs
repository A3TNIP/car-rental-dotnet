using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface IKhaltiService
{
    public Task<BaseResponseDto<KhaltiPaymentResponse>> VerifyPayment(KhaltiPaymentRequestDTO dto);
    public Task<BaseResponseDto<KhaltiInitiateResponse>> InitiateTransaction(KhaltiInitiateRequest initiateRequestDto);
    public Task<BaseResponseDto<KhaltiPaymentResponse>> ConfirmTransaction(KhaltiConfirmationRequestDto paymentRequest);
}
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Infrastructure.Service;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhaltiPaymentController : ControllerBase
    {
        private readonly IKhaltiService _khaltiPaymentService;

        public KhaltiPaymentController(IKhaltiService khaltiPaymentService)
        {
            _khaltiPaymentService = khaltiPaymentService;
        }

        /// <summary>
        /// Initiates a transaction with Khalti payment service.
        /// </summary>
        /// <param name="initiateRequestDto">The request DTO containing necessary details for initiating a transaction.</param>
        /// <returns>The response DTO containing transaction details.</returns>
        [HttpPost("initiate-transaction")]
        public async Task<BaseResponseDto<KhaltiInitiateResponse>> InitiateTransaction(
            [FromBody] KhaltiInitiateRequest initiateRequestDto)
        {
            return await _khaltiPaymentService.InitiateTransaction(initiateRequestDto);
        }

        /// <summary>
        /// Verifies a payment with Khalti payment service.
        /// </summary>
        /// <param name="paymentRequest">The request DTO containing necessary details for verifying a payment.</param>
        /// <returns>The response DTO containing payment details.</returns>
        [HttpPost("verify-payment")]
        public async Task<BaseResponseDto<KhaltiPaymentResponse>> VerifyPayment([FromBody] KhaltiPaymentRequestDTO paymentRequest)
        {
            return await _khaltiPaymentService.VerifyPayment(paymentRequest);
        }
        
        /// <summary>
        /// Confirms a transaction with Khalti payment service.
        /// </summary>
        /// <param name="paymentRequest">The request DTO containing necessary details for confirming a transaction.</param>
        /// <returns>The response DTO containing transaction details.</returns>
        [HttpPost("ConfirmTransaction")]
        public async Task<BaseResponseDto<KhaltiPaymentResponse>> ConfirmTransaction([FromBody] KhaltiConfirmationRequestDto paymentRequest)
        {
            return await _khaltiPaymentService.ConfirmTransaction(paymentRequest);
        }
    }
}
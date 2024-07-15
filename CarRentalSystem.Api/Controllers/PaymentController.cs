using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

public class PaymentController: BaseController<Payment, PaymentDto>
{
    private readonly IPaymentService _paymentService;
    public PaymentController(BaseService<Payment, PaymentDto> baseService, IPaymentService paymentService) : base(baseService)
    {
        _paymentService = paymentService;
    }
    
    
    /// <summary>
    /// Get sum of total amount for authenticated user
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    [HttpGet("GetSumOfTotalAmount")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<BaseResponseDto<decimal>> GetSumOfTotalAmount()
    {
        return await _paymentService.GetSumOfTotalAmount();
    }
    
    /// <summary>
    /// Get payments by date for admin or staff users
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    [HttpGet("PaymentsByDate")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, Staff")]
    public async Task<BaseResponseDto<decimal>> GetPaymentsByDate()
    {
        return await _paymentService.GetPaymentChart();
    }
}
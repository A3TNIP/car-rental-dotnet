using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

public class BillController: BaseController<Bill, BillDto>
{
    private readonly IBillService _billService;
    public BillController(BaseService<Bill, BillDto> baseService, IBillService billService) : base(baseService)
    {
        _billService = billService;
    }

    /// <summary>
    /// Get the latest bill for the authenticated user
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Latest bill DTO</returns>
    [HttpGet("Latest")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<BaseResponseDto<BillDto>> GetLatest(HttpContext context)
    {
        return await _billService.GetLatestBill(context);
    }
    
    /// <summary>
    /// Generate a new bill
    /// </summary>
    /// <param name="billDto">Bill DTO</param>
    /// <returns>Generated bill DTO</returns>
    [HttpPost("Generate")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin, Staff")]
    public async Task<BaseResponseDto<BillDto>> GenerateBill([FromBody] BillDto billDto)
    {
        return await _billService.GenerateBill(billDto);
    }
    
    /// <summary>
    /// Get the bill for a specific rental
    /// </summary>
    /// <param name="id">Rental ID</param>
    /// <returns>Bill DTO</returns>
    [HttpGet("Rent/{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<BaseResponseDto<BillDto>> GetBillByRentId(string id)
    {
        return await _billService.GetBillByRentId(id);
    }
    
    [HttpGet("Sales")]
    public async Task<BaseResponseDto<SalesDto>> GetSales()
    {
        return await _billService.GetSales();
    }
    
    [HttpPost("SalesFilteredList")]
    public async Task<BaseResponseDto<SalesDto>> GetSalesFilteredList([FromBody] SalesFilterDto dto)
    {
        return await _billService.GetSalesFilteredList(dto);
    }
}
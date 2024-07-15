using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

public class DamageController: BaseController<Damage, DamageDto>
{
    private readonly IDamageService _damageService;
    public DamageController(BaseService<Damage, DamageDto> baseService, IDamageService damageService) : base(baseService)
    {
        _damageService = damageService;
    }
    
    /// <summary>
    /// Completes a damage report for a car by an authorized user
    /// </summary>
    /// <param name="damageDto"></param>
    /// <returns></returns>
    [HttpPut("complete")]
    [Authorize(Roles = "Admin, Staff", AuthenticationSchemes = "Bearer")]
    public async Task<BaseResponseDto<DamageDto>> CompleteDamageReport(DamageDto damageDto)
    {
        return await _damageService.CompleteDamageReport(damageDto, HttpContext);
    }

    /// <summary>
    /// Returns the count of damage reports
    /// </summary>
    /// <returns></returns>
    [HttpGet(template:"DamageCount")]
    public async Task<BaseResponseDto<int>> DamageReportCount()
    {
        return await _damageService.DamageReportCount();
    }
    
    [HttpGet("Rent/{rentId}")]
    public async Task<BaseResponseDto<DamageDto>> GetDamageByRentId(Guid rentId)
    {
        return await _damageService.GetDamageByRentId(rentId);
    }
}
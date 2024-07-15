using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface IDamageService
{
    public Task<BaseResponseDto<DamageDto>> CompleteDamageReport(DamageDto damageDto, HttpContext context);

    public Task<BaseResponseDto<int>> DamageReportCount();
    Task<BaseResponseDto<DamageDto>> GetDamageByRentId(Guid rentId);
}
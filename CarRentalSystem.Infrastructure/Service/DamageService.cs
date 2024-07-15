using System.Security.Claims;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Service;

public class DamageService: BaseService<Damage, DamageDto>, IDamageService
{
    private readonly ApplicationDbContext _context;
    public DamageService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Complete damage report for a car
    /// </summary>
    /// <param name="damageDto">Damage DTO object</param>
    /// <param name="context">HTTP context object</param>
    /// <returns>Response DTO with damage details</returns>
    public async Task<BaseResponseDto<DamageDto>> CompleteDamageReport(DamageDto damageDto, HttpContext context)
    {
        // check if damage report exists
        if (damageDto.RepairCost == null)
        {
            throw new DomainException("Repair cost is required", 400);
        }

        // get user id
        var userEmail = context.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        var userId = await (from u in _context.Users where u.Email == userEmail select u.Id).FirstAsync();

        // get damage report
        var damage = await (from d in _context.Damages where d.Id == damageDto.DamageId select d).FirstAsync();

        // set damage report
        damage.EmployeeId = userId;
        damage.RepairCost = damageDto.RepairCost;

        // update damage report
        var dbInstance = _context.Set<Damage>().Update(damage);
        await _context.SaveChangesAsync();

        // return damage report
        return new BaseResponseDto<DamageDto>(new DamageDto().MapToDto(dbInstance.Entity));
    }

    /// <summary>
    /// Get the count of total damage reports
    /// </summary>
    /// <returns>Response DTO with count of damage reports</returns>
    public async Task<BaseResponseDto<int>> DamageReportCount()
    {
        var reportCount = await(from damages in _context.Damages
            select damages).CountAsync();
        return new BaseResponseDto<int>(reportCount);
    }

    public async Task<BaseResponseDto<DamageDto>> GetDamageByRentId(Guid rentId)
    {
        var damages = await (from d in _context.Damages where d.RentalId == rentId select d).FirstAsync();
        if (damages == null)
        {
            return new BaseResponseDto<DamageDto>(true, "No damage report found");
        }
        return new BaseResponseDto<DamageDto>(false, "Damage report found");
    }
}
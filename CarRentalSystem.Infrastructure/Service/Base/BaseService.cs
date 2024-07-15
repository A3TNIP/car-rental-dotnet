using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Base;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Service.Base;

public abstract class BaseService<TE, TD> where TE : BaseEntity where TD : IBaseDto<TE, TD>, new()
{
    private readonly ApplicationDbContext _context;

    protected BaseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponseDto<TD>> GetAll() =>
        new (await _context.Set<TE>().Where(entity => entity.ActiveStatus == true).Select(entity => new TD().MapToDto(entity)).ToListAsync());


    public async Task<BaseResponseDto<TD>> GetById(Guid id)
    {
        var entity = await _context.Set<TE>().Where(e => e.Id == id).Select(entity => new TD().MapToDto(entity))
            .FirstOrDefaultAsync();
        if (entity == null)
        {
            throw new NotFoundException($"Entity not found with id {id}");
        }
        return new (entity);
    }

    public async Task<BaseResponseDto<TD>> Create(TD dto)
    {
        var entity = dto.MapToEntity();
        entity.ActiveStatus = true;
        var dbInstance = await _context.Set<TE>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return new(new TD().MapToDto(dbInstance.Entity));
    }

    public async Task<BaseResponseDto<TD>> Update(TD dto)
    {
        var entity = _context.Set<TE>().First(e => e.Id == dto.GetId());
        entity = dto.UpdateEntity(entity, dto);
        var dbInstance = _context.Set<TE>().Update(entity);
        await _context.SaveChangesAsync();
        return new (new TD().MapToDto(dbInstance.Entity));
    }

    public async Task<BaseResponseDto<TD>> Delete(Guid id)
    {
        var entity = await _context.Set<TE>().Where(e => e.Id == id).FirstAsync();
        entity.ActiveStatus = false;
        _context.Set<TE>().Update(entity);
        await _context.SaveChangesAsync();
        return new BaseResponseDto<TD>(false, "Deleted successfully");
    }
}
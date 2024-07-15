using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.DTos;

public class DamageDto: IBaseDto<Damage, DamageDto>
{
    public Guid? DamageId { get; set; }
    public Guid RentalId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string DamagedParts { get; set; }
    public string? EmployeeId { get; set; }
    public decimal? RepairCost { get; set; }
    
    public DamageDto MapToDto(Damage entity)
    {
        return new()
        {
            DamageId = entity.Id,
            RentalId = entity.RentalId,
            Date = entity.Date,
            Description = entity.Description,
            DamagedParts = entity.DamagedParts,
            EmployeeId = entity.EmployeeId,
            RepairCost = entity.RepairCost,
        };
    }

    public Damage MapToEntity()
    {
        return new()
        {
            Id = DamageId ?? Guid.NewGuid(),
            RentalId = RentalId,
            Date = Date,
            Description = Description,
            DamagedParts = DamagedParts,
        };
    }

    public Damage UpdateEntity(Damage entity, DamageDto dto)
    {
        entity.Date = dto.Date;
        entity.Description = dto.Description;
        entity.DamagedParts = dto.DamagedParts;
        entity.EmployeeId = dto.EmployeeId;
        entity.RepairCost = dto.RepairCost;
        return entity;
    }

    public Guid GetId()
    {
        return DamageId ?? throw new InvalidOperationException("Id cannot be null.");
    }

    public string GetName()
    {
        return $"Damage Report {Date} - {RentalId}";
    }
}
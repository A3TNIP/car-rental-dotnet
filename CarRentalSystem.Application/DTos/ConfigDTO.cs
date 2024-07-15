using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.DTos;

public class ConfigDTO: IBaseDto<Config, ConfigDTO>
{
    public Guid? Id { get; set; }
    public string Code { get; set; }
    public string Key { get; set; }
    public string? Value { get; set; }
    public ConfigDTO MapToDto(Config entity)
    {
        return new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Key = entity.Key,
            Value = entity.Value
        };
    }

    public Config MapToEntity()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Code = Code,
            Key = Key,
            Value = Value!
        };
    }

    public Config UpdateEntity(Config entity, ConfigDTO dto)
    {
        entity.Code = dto.Code;
        entity.Key = dto.Key;
        entity.Value = dto.Value!;
        return entity;
    }

    public Guid GetId()
    {
        return Id ?? throw new InvalidOperationException("Id cannot be found");
    }

    public string GetName()
    {
        return "";
    }
}
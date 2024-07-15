using CarRentalSystem.Domain.Base;

namespace CarRentalSystem.Domain.Entities;

public class Config: BaseEntity
{
    public string Code { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
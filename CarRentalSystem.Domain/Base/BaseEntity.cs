using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Base;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastModifiedOn { get; set; }
    public bool ActiveStatus { get; set; }
}
using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.DTos;

public class BillDto: IBaseDto<Bill, BillDto>
{
    public Guid? BillId { get; set; }
    public Guid RentalId { get; set; }
    public DateTime GeneratedDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? RepairCost { get; set; }
    public decimal? Rate { get; set; }
    public decimal? Discount { get; set; }
    public int days { get; set; }
    public string? BillFor { get; set; }
    public string? CustomerId { get; set; }
    public BillDto MapToDto(Bill entity)
    {
        return new()
        {
            BillId = entity.Id,
            RentalId = entity.RentalId,
            GeneratedDate = entity.CreatedOn,
            DueDate = entity.DueDate,
            TotalAmount = entity.TotalAmount,
            RepairCost = entity.RepairCost,
            Rate = entity.Rate,
            Discount = entity.Discount,
            days = entity.days,
            BillFor = entity.Rent!.RequestedBy.Name,
            CustomerId = entity.Rent.RequestedBy.Id,
        };
    }

    public Bill MapToEntity()
    {
        return new()
        {
            Id = BillId ?? Guid.NewGuid(),
            RentalId = RentalId,
            DueDate = DueDate,
            TotalAmount = TotalAmount ?? 0,
            ActiveStatus = true
        };
    }

    public Bill UpdateEntity(Bill entity, BillDto dto)
    {
        entity.DueDate = dto.DueDate;
        entity.TotalAmount = dto.TotalAmount ?? 0;
        return entity;
    }

    public Guid GetId()
    {
        return BillId ?? throw new InvalidOperationException("Id cannot be null.");
    }

    public string GetName()
    {
        return $"Bill for rental {RentalId}";
    }
}
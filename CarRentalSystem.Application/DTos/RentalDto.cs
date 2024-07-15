using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Application.DTos;

public class RentalDto: IBaseDto<Rent, RentalDto>
{
    public Guid? Id { get; set; }
    public string? RequestedBy { get; set; }
    public Guid CarId { get; set; }
    public string? CarName { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
    public Guid? OfferId { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalPrice { get; set; }
    public RentalDto MapToDto(Rent entity)
    {
        return new()
        {
            Id = entity.Id,
            RequestedBy = entity.RequestedBy != null ? entity.RequestedBy.Name : "No Name",
            CarId = entity.CarId,
            CarName = $"{entity.Car.Brand} {entity.Car.Model}",
            ApprovedBy = entity.ApprovedBy != null ? entity.ApprovedBy.Name : "Not Approved",
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            OfferId = entity.OfferId,
            Status = entity.Status.ToString()
        };
    }

    public Rent MapToEntity()
    {
        return new Rent
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = EndDate,
            Status = RentalStatus.Waiting,
            OfferId = OfferId,
            Car = new Car
            {
                Id = CarId
            }
        };
    }

    public Rent UpdateEntity(Rent entity, RentalDto dto)
    {
        entity.Status = dto.Status!.ToUpper() switch
        {
            "WAITING" => RentalStatus.Waiting,
            "APPROVED" => RentalStatus.Approved,
            "REJECTED" => RentalStatus.Rejected,
            "CANCELLED" => RentalStatus.Cancelled,
            "COMPLETED" => RentalStatus.Completed,
            _ => entity.Status
        };
        return entity;
    }

    public Guid GetId()
    {
        return Id ?? Guid.Empty;
    }

    public string GetName()
    {
        return $"${RequestedBy} - {CarId} - {StartDate} - {EndDate}";
    }
}
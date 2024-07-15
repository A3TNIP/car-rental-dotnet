using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Application.DTos;

public class CarDto : IBaseDto<Car, CarDto>
{
    public Guid? CarId { get; set; }
    public string Model { get; set; }
    public string Make { get; set; }
    public string Color { get; set; }
    public string LicensePlate { get; set; }
    public int BuildYear { get; set; }
    public string Brand { get; set; }
    public Decimal Rate { get; set; }
    public decimal DiscountedRate { get; set; }
    public decimal? DiscountModifier { get; set; }
    public Guid? OfferId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ImageUrl { get; set; }
    public List<DateTime>? RentedDates { get; set; }


    public Guid GetId()
    {
        if (CarId == null)
        {
            throw new InvalidOperationException("CarId cannot be null.");
        }

        return CarId.Value;
    }

    public string GetName()
    {
        return $"{BuildYear} {Brand} {Model}";
    }

    public CarDto MapToDto(Car entity)
    {
        return new CarDto()
        {
            CarId = entity.Id,
            Brand = entity.Brand,
            LicensePlate = entity.LicensePlate,
            BuildYear = entity.BuildYear.Year,
            Color = entity.Color,
            CreatedOn = entity.CreatedOn,
            Make = entity.Make,
            Model = entity.Model,
            Rate = entity.Rate,
            DiscountModifier = 0,
            Status = MapStatusToString(entity.Status),
            ImageUrl = entity.ImageUrl ?? ""
        };
    }

    public Car MapToEntity()
    {
        return new Car()
        {
            Brand = Brand,
            BuildYear = new DateOnly(BuildYear, 1, 1),
            Color = Color,
            CreatedOn = CreatedOn,
            LicensePlate = LicensePlate,
            Id = CarId ?? Guid.NewGuid(),
            Make = Make,
            Model = Model,
            Rate = Rate,
            Status = MapStringToStatus(Status)
        };
    }

    public Car UpdateEntity(Car entity, CarDto dto)
    {
        entity.Brand = dto.Brand;
        entity.BuildYear = new DateOnly(BuildYear, 1, 1);
        entity.Color = dto.Color;
        entity.LicensePlate = LicensePlate;
        entity.Make = dto.Make;
        entity.Model = dto.Model;
        entity.Rate = dto.Rate;
        entity.Status = MapStringToStatus(dto.Status);
        return entity;
    }

    private CarStatus MapStringToStatus(string status)
    {
        switch (status)
        {
            case "Available":
                return CarStatus.Available;
            case "Rented":
                return CarStatus.Rented;
            case "In Service":
                return CarStatus.InService;
            case "Reserved":
                return CarStatus.Reserved;
            case "Damaged":
                return CarStatus.Damaged;
            default:
                throw new ArgumentException($"Invalid Status value: {status}");
        }
    }

    private string MapStatusToString(CarStatus status)
    {
        switch (status)
        {
            case CarStatus.Available:
                return "Available";
            case CarStatus.Rented:
                return "Rented";
            case CarStatus.InService:
                return "In Service";
            case CarStatus.Reserved:
                return "Reserved";
            case CarStatus.Damaged:
                return "Damaged";
            default:
                throw new ArgumentException($"Invalid value for CarStatus: {status}", nameof(status));
        }
    }
}
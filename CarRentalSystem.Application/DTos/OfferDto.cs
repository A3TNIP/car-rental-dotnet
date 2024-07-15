using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Application.DTos;

public class OfferDto: IBaseDto<Offer, OfferDto>
{
    public Guid? Id { get; set; }
    public string OfferName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Discount { get; set; }
    public string Type { get; set; }
    public string? OfferDescription { get; set; }
    public OfferDto MapToDto(Offer entity)
    {
        var customerType = entity.Type == OfferType.Closed ? "all our Active" : "all our";
        return new()
        {
            Id = entity.Id,
            OfferName = entity.OfferName,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Discount = entity.Discount,
            Type = entity.Type.ToString(),
            OfferDescription = $"For {customerType} users we are offering {entity.Discount}% discount on all cars. between {entity.StartDate:D} and {entity.EndDate:D}"
        };
    }

    public Offer MapToEntity()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            OfferName = OfferName,
            StartDate = StartDate,
            EndDate = EndDate,
            Discount = Discount,
            Type = Enum.Parse<OfferType>(Type)
        };
    }

    public Offer UpdateEntity(Offer entity, OfferDto dto)
    {
        entity.OfferName = dto.OfferName;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Discount = dto.Discount;
        entity.Type = Enum.Parse<OfferType>(dto.Type);
        return entity;
    }

    public Guid GetId()
    {
        return Id ?? throw new InvalidOperationException("Id is null");
    }

    public string GetName()
    {
        return OfferName;
    }
}
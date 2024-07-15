using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.DTos;

public class PaymentDto: IBaseDto<Payment, PaymentDto>
{
    public Guid? PaymentId { get; set; }
    public Guid BillId { get; set; }
    public decimal PaidAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CustomerId { get; set; }
    public string? EmployeeId { get; set; }
    public PaymentDto MapToDto(Payment entity)
    {
        return new()
        {
            PaymentId = entity.Id,
            BillId = entity.BillId,
            PaidAmount = entity.PaidAmount,
            PaymentMethod = entity.PaymentMethod,
            CustomerId = entity.CustomerId,
            EmployeeId = entity.EmployeeId,
        };
    }

    public Payment MapToEntity()
    {
        return new()
        {
            Id = PaymentId ?? Guid.NewGuid(),
            BillId = BillId,
            PaidAmount = PaidAmount,
            PaymentMethod = PaymentMethod ?? "Manual",
            CustomerId = CustomerId ?? throw new InvalidOperationException("Customer Id cannot be null."),
        };
    }

    public Payment UpdateEntity(Payment entity, PaymentDto dto)
    {
        entity.EmployeeId = dto.EmployeeId;
        entity.PaidAmount = dto.PaidAmount;
        entity.PaymentMethod = dto.PaymentMethod;
        return entity;
    }

    public Guid GetId()
    {
        return PaymentId ?? throw new InvalidOperationException("Id cannot be null.");
    }

    public string GetName()
    {
        return $"Payment for bill {BillId}";
    }
}
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Service;

public class PaymentService: BaseService<Payment, PaymentDto>, IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets the sum of total amount of payments made.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the sum of total amount.</returns>
    public async Task<BaseResponseDto<decimal>> GetSumOfTotalAmount()
    {
        var totalAmountSum = await _context.Bills
            .SumAsync(b => b.TotalAmount);

        return new BaseResponseDto<decimal>(totalAmountSum);
    }

    /// <summary>
    /// Gets the payment chart data for visualization.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the payment chart data.</returns>
    public async Task<BaseResponseDto<decimal>> GetPaymentChart()
    {
        var result = await _context.Payments
            .GroupBy(p => p.CreatedOn.Date)
            .Select(g => g.Sum(p => p.PaidAmount))
            .ToListAsync();
        return new BaseResponseDto<decimal>(result);
    }
}
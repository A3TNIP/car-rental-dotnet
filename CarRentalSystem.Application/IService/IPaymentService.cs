using CarRentalSystem.Application.Base;

namespace CarRentalSystem.Application.IService;

public interface IPaymentService
{
    public Task<BaseResponseDto<decimal>> GetSumOfTotalAmount();
    public Task<BaseResponseDto<decimal>> GetPaymentChart();
}
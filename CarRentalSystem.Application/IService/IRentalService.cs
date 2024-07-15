using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface IRentalService
{
    public Task<BaseResponseDto<RentalDto>> RequestRental(RentalDto dto, HttpContext context);
    Task<BaseResponseDto<RentalDto>> ChangeStatus(RentalDto dto, HttpContext httpContext);
    Task<BaseResponseDto<RentalDto>> GetByUser(HttpContext context);

    public Task<BaseResponseDto<int>> GetAllRentsCount();
    Task<BaseResponseDto<RentalDto>> GetLatestRental(HttpContext httpContext);
    public Task<BaseResponseDto<CarDto>> GetMostRentedCar();
    public Task<BaseResponseDto<CarDto>> GetLeastRentedCar();
}
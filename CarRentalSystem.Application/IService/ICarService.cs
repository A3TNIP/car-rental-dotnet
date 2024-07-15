using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface ICarService
{
    public Task<BaseResponseDto<CarDto>> GetCarsWithDiscountAsync(HttpContext context);
    Task<BaseResponseDto<CarDto>> UploadImage(IFormFile image, string carId, HttpContext context);
    public Task<BaseResponseDto<int>> GetCarsTotalCount();

    public Task<BaseResponseDto<int>> GetCarsAvailableCount();
    
    public Task<BaseResponseDto<int>> GetCarsOnRentCount();
    public Task<BaseResponseDto<CarDto>> GetMostRentedCar();


}
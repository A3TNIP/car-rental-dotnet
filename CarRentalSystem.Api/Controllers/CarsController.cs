using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
public class CarsController: BaseController<Car, CarDto>
{
    private readonly ICarService _carService;
    public CarsController(BaseService<Car, CarDto> baseService, ICarService carService) : base(baseService)
    {
        _carService = carService;
    }
    
    [HttpGet("AfterDiscount")]
    [AllowAnonymous]
    public async Task<BaseResponseDto<CarDto>> GetCarsWithDiscountAsync()
    {
        return await _carService.GetCarsWithDiscountAsync(HttpContext);
    }
    
    /// <summary>
    /// Uploads an image for a car.
    /// </summary>
    /// <param name="image">The image file to upload.</param>
    /// <param name="carId">The ID of the car to upload the image for.</param>
    [HttpPost("UploadImage")]
    [Authorize(Roles = "Admin, staff", AuthenticationSchemes = "Bearer")]
    public async Task<BaseResponseDto<CarDto>> UploadImage([FromForm] IFormFile image, [FromForm] string carId)
    {
        return await _carService.UploadImage(image, carId, HttpContext);
    }
    
    /// <summary>
    /// Gets the total count of all cars.
    /// </summary>
    [HttpGet("TotalCarCount")]
    public async Task<BaseResponseDto<int>> GetCarsTotalCount()
    {
        return await _carService.GetCarsTotalCount();
    }
    
    /// <summary>
    /// Gets the count of available cars.
    /// </summary>
    [HttpGet("CarsAvailableCount")]
    public async Task<BaseResponseDto<int>> GetCarsAvailableCount()
    {
        return await _carService.GetCarsAvailableCount();
    }
    
    /// <summary>
    /// Gets the count of cars currently on rent.
    /// </summary>
    [HttpGet("CarsOnRentCount")]
    public async Task<BaseResponseDto<int>> GetCarsOnRentCount()
    {
        return await _carService.GetCarsOnRentCount();
    }
    
    /// <summary>
    /// Get sales for each car
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetMostRentedCar")]
    public async Task<BaseResponseDto<CarDto>> GetMostRentedCar()
    {
        return await _carService.GetMostRentedCar();
    }
}
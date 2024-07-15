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
public class RentalController: BaseController<Rent, RentalDto>
{
    private readonly IRentalService _rentalService;
    public RentalController(BaseService<Rent, RentalDto> baseService, IRentalService rentalService) : base(baseService)
    {
        _rentalService = rentalService;
    }
    
    // Endpoint for requesting a rental
    [HttpPost("Request")]
    public async Task<BaseResponseDto<RentalDto>> RequestRental([FromBody] RentalDto dto)
    {
        return await _rentalService.RequestRental(dto, HttpContext);
    }
    
    // Endpoint for changing rental status
    [HttpPost("ChangeStatus")]
    public async Task<BaseResponseDto<RentalDto>> ChangeStatus([FromBody] RentalDto dto)
    {
        return await _rentalService.ChangeStatus(dto, HttpContext);
    }
    
    // Endpoint for getting rentals by user
    [HttpGet("User")]
    public async Task<BaseResponseDto<RentalDto>> GetByUser()
    {
        return await _rentalService.GetByUser(HttpContext);
    }

    // Endpoint for getting total count of rentals
    [HttpGet("CountRent")]
    public async Task<BaseResponseDto<int>> GetAllRentsCount()
    {
        return await _rentalService.GetAllRentsCount();
    }
    
    // Endpoint for getting the latest rental
    [HttpGet("Latest")]
    public async Task<BaseResponseDto<RentalDto>> GetLatest()
    {
        return await _rentalService.GetLatestRental(HttpContext);
    }
    
    // Endpoint for getting the most rented car
    [HttpGet("MostRentedCar")]
    public async Task<BaseResponseDto<CarDto>> GetMostRentedCar()
    {
        return await _rentalService.GetMostRentedCar();
    }
    
    // Endpoint for getting the least rented car
    [HttpGet("LeastRentedCar")]
    public async Task<BaseResponseDto<CarDto>> GetLeastRentedCar()
    {
        return await _rentalService.GetLeastRentedCar();
    }
}

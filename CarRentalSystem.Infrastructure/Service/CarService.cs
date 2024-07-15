using System.Data;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Service.Base;
using CarRentalSystem.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarRentalSystem.Infrastructure.Service;

public class CarService : BaseService<Car, CarDto>, ICarService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IAttachmentService _attachmentService;

    public CarService(ApplicationDbContext context, UserManager<User> userManager,
        IAttachmentService attachmentService) : base(context)
    {
        _context = context;
        _userManager = userManager;
        _attachmentService = attachmentService;
    }

    /// <summary>
    /// Get cars with discount
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<BaseResponseDto<CarDto>> GetCarsWithDiscountAsync(HttpContext context)
    {
        // Get user
        var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null)
        {
            var data = await (from c in _context.Cars select new CarDto().MapToDto(c)).ToListAsync();
            return new BaseResponseDto<CarDto>(data);
        }
        var user = await _userManager.FindByEmailAsync(userEmail) ?? throw new DomainException("User not found", 400);

        // Create connection for stored procedure call
        await using var npgConnection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await npgConnection.OpenAsync();
        await using var cmd = new NpgsqlCommand("get_car_list", npgConnection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue(user.Id);

        // Map the result set to dto
        await using var reader = await cmd.ExecuteReaderAsync();
        var carList = await _context.Set<Car>().Select(c => new CarDto().MapToDto(c)).ToListAsync();
        while (await reader.ReadAsync())
        {
            var id = reader.GetGuid(0);
            var discountRate = reader.GetDecimal(4);
            var dto = carList.First(c => c.CarId == id);
            dto.DiscountedRate = discountRate;
            dto.DiscountModifier = reader.GetDecimal(3);
            if (!reader.IsDBNull(1))
            {
                dto.OfferId = reader.GetGuid(1);
            }
            else
            {
                dto.OfferId = null;
            }
        }

        // Get the approved rents
        var rentsApproved = await (from rent in _context.Rents
                where rent.Status == RentalStatus.Approved
                      && rent.StartDate >= DateTime.UtcNow
                select rent)
            .ToListAsync();

        // Add the rented dates to the dto
        foreach (var rent in rentsApproved)
        {
            var carDto = carList.First(car => car.CarId == rent.CarId);
            carDto.RentedDates = new List<DateTime>();
            carDto.RentedDates.AddRange(DateHelper.GetDatesInRange(rent.StartDate, rent.EndDate));
        }

        // return the dto
        return new BaseResponseDto<CarDto>(carList);
    }

    /// <summary>
    /// Upload image for car
    /// </summary>
    /// <param name="image"></param>
    /// <param name="carId"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<BaseResponseDto<CarDto>> UploadImage(IFormFile image, string carId, HttpContext context)
    {
        // Get the car from the database
        var car = await (from c in _context.Cars where c.Id == Guid.Parse(carId) select c).FirstOrDefaultAsync()
                  ?? throw new DomainException("Car not found", 400);

        // Get the user's email from the claims
        var userEmail = context.User.FindFirst(ClaimTypes.Email)!.Value;

        // Upload the image
        var imageUploadResponse = await _attachmentService.Upload(image, userEmail, context);

        // Update the car with the new image URL
        if (imageUploadResponse.IsSuccess)
        {
            car.ImageUrl = imageUploadResponse.Data.FileUrl;
            _context.Set<Car>().Update(car);
            await _context.SaveChangesAsync();
            return new BaseResponseDto<CarDto>(new CarDto().MapToDto(car));
        }

        // If the image upload failed, throw a domain exception
        throw new DomainException("Image upload failed", 400);
    }

    /// <summary>
    /// Get total count of cars
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseDto<int>> GetCarsTotalCount()
    {
        var totalcount = await (from car in _context.Cars
            select car).CountAsync();
        return new BaseResponseDto<int>(totalcount);
    }

    /// <summary>
    /// Get count of available cars
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseDto<int>> GetCarsAvailableCount()
    {
        var carsAvailableCount = await (from car in _context.Cars
            where car.Status == CarStatus.Available
            select car).CountAsync();
        return new BaseResponseDto<int>(carsAvailableCount);
    }

    /// <summary>
    /// Get count of cars on rent
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseDto<int>> GetCarsOnRentCount()
    {
        var carsOnRentCount = await (from car in _context.Cars
            where car.Status == CarStatus.Rented
            select car).CountAsync();
        return new BaseResponseDto<int>(carsOnRentCount);
    }

    public async Task<BaseResponseDto<CarDto>> GetMostRentedCar()
    {
        var car = await (from r in _context.Rents
            join c in _context.Cars on r.CarId equals c.Id 
            where c.Status == CarStatus.Available 
            select r).ToListAsync();
        var mostRentedGuid = car.GroupBy(r => r.CarId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();
        var mostRented = await (from c in _context.Cars where c.Id == mostRentedGuid select c).FirstOrDefaultAsync() ?? throw new DomainException("Car Not Found", 400);
        return new BaseResponseDto<CarDto>(new CarDto().MapToDto(mostRented));
    }
}
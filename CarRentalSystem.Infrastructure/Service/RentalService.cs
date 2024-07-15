using System.Security.Claims;
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

namespace CarRentalSystem.Infrastructure.Service;

public class RentalService : BaseService<Rent, RentalDto>, IRentalService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IEmailProvider _emailProvider;

    public RentalService(ApplicationDbContext context, UserManager<User> userManager, IEmailProvider gmailEmailProvider) : base(context)
    {
        _context = context;
        _userManager = userManager;
        _emailProvider = gmailEmailProvider;
    }

    /// <summary>
    /// Requests a rental and returns a RentalDto
    /// </summary>
    /// <param name="dto">RentalDto object containing rental information</param>
    /// <param name="context">HttpContext containing user information</param>
    /// <returns>A BaseResponseDto containing the RentalDto object</returns>
    public async Task<BaseResponseDto<RentalDto>> RequestRental(RentalDto dto, HttpContext context)
    {
        // Check if the car is already rented in the requested period
        var isCarRented = await (from r in _context.Rents
            where r.CarId == dto.CarId && r.Status == RentalStatus.Approved &&
                  ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
                   (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate))
            select r).AnyAsync();

        if (isCarRented) throw new DomainException("Car is already rented in this period", 400);
      
        var entity = dto.MapToEntity();
        entity.Car = await _context.Set<Car>().FirstOrDefaultAsync(c => c.Id == dto.CarId) ??
                     throw new DomainException("Car not found", 400);
        entity.Status = RentalStatus.Waiting;
        entity.ActiveStatus = true;

        var userEmail = context.User.FindFirst(ClaimTypes.Email)!.Value;
        var requestedBy =
            await _userManager.FindByEmailAsync(userEmail) ?? throw new DomainException("User not found", 400);
        entity.RequestedById = requestedBy.Id;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        var dbInstance = await _context.Set<Rent>().AddAsync(entity);
        await _context.SaveChangesAsync();

        var rentalDto = new RentalDto().MapToDto(dbInstance.Entity);
        rentalDto.Discount = await (from o in _context.Offers where o.Id == dto.OfferId select o.Discount).FirstOrDefaultAsync();
        rentalDto.TotalPrice = ((dto.EndDate - dto.StartDate).Days * entity.Car.Rate) * ((100 - rentalDto.Discount)/ 100);
        if (!_userManager.IsInRoleAsync(requestedBy,"Customer").Result)
        {
            rentalDto.TotalPrice -= (rentalDto.TotalPrice * 10/100);
        }
        return new BaseResponseDto<RentalDto>(rentalDto);
    }

    /// <summary>
    /// Changes the status of a rental and returns a RentalDto
    /// </summary>
    /// <param name="dto">RentalDto object containing rental information</param>
    /// <param name="httpContext">HttpContext containing user information</param>
    /// <returns>A BaseResponseDto containing the RentalDto object</returns>
    public async Task<BaseResponseDto<RentalDto>> ChangeStatus(RentalDto dto, HttpContext httpContext)
    {
        var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)!.Value;
        var user = _userManager.FindByEmailAsync(userEmail).Result ?? throw new DomainException("User not found", 400);
        if (!(_userManager.IsInRoleAsync(user, "Admin").Result || _userManager.IsInRoleAsync(user, "Staff").Result))
        {
            if (dto.Status != "Cancelled") throw new DomainException("You are not allowed to change the status of this rental", 403);
        }
        
        var rental = _context.Set<Rent>().FirstOrDefault(r => r.Id == dto.Id) ?? throw new DomainException("Rental not found", 400);
        
        if (rental.Status == RentalStatus.Completed) throw new DomainException("Rental already completed", 400);
        if (rental.Status == RentalStatus.Cancelled) throw new DomainException("Rental already cancelled", 400);
        if (rental.Status == RentalStatus.Approved) throw new DomainException("Rental already approved", 400);

        if (dto.Status!.ToUpper() == "APPROVED")
        {
            var rentedDates = new List<DateTime>();
            // check if car is available in the requested period
            var rents = await (from r in _context.Rents 
                where r.CarId == rental.CarId 
                && r.Status == RentalStatus.Approved 
                select r).ToListAsync();
            foreach (var rent in rents)
            {
                rentedDates.AddRange(DateHelper.GetDatesInRange(rent.StartDate, rent.EndDate));
            }
            var requestedDates = DateHelper.GetDatesInRange(rental.StartDate, rental.EndDate);
            var isRented = requestedDates.Any(d => rentedDates.Contains(d));
            if (isRented)
            {
                throw new DomainException("Car is not available in the requested period", 400);
            }
            rental.ApprovedById = user.Id;
        }
        rental.Status = Enum.Parse<RentalStatus>(dto.Status);
        var updated = _context.Set<Rent>().Update(rental);
        await _context.SaveChangesAsync();
        var car = await (from c in _context.Cars where c.Id == rental.CarId select c).FirstAsync();
        var reciever = await (from u in _context.Users where u.Id == rental.RequestedById select u).FirstAsync();
        var message = new EmailMessage {
            Subject = "Rent Request Approved", 
            To = reciever.Email, 
            Body = @$"Dear {reciever.Name},
We are delighted to inform you that your rental request has been processed and your car is now available for pick up.

The car details are as follows:

Model: {car.Model}
Make: {car.Make}
Color: {car.Color}
License Plate: {car.LicensePlate}
Brand: {car.Brand}

Please make sure to bring a valid identification document.

Thank you for choosing our rental service. We hope you have a safe and enjoyable ride.

Best regards,
Hajurko Car Rental"
        };

        await _emailProvider.SendEmailAsync(message);
        return new BaseResponseDto<RentalDto>(new RentalDto().MapToDto(updated.Entity));
    }

    /// <summary>
    /// Gets all rentals of a user and returns a RentalDto
    /// </summary>
    /// <param name="context">HttpContext containing user information</param>
    /// <returns>A BaseResponseDto containing the RentalDto object</returns>
    public async Task<BaseResponseDto<RentalDto>> GetByUser(HttpContext context)
    {
        var userEmail = context.User.FindFirst(ClaimTypes.Email)!.Value;
        var rents = await (from r in _context.Rents join u in _context.Users on r.RequestedById equals u.Id 
                where u.Email == userEmail select new RentalDto().MapToDto(r)).ToListAsync();
        return new BaseResponseDto<RentalDto>(rents);
    }
    
    /// <summary>
    /// Gets the count of all rentals and returns an integer
    /// </summary>
    /// <returns>A BaseResponseDto containing the integer value of the count</returns>
    public async Task<BaseResponseDto<int>> GetAllRentsCount()
    {
        var totalcount = await (from rents in _context.Rents
            where rents.Status == RentalStatus.Completed
            select rents).CountAsync();
        return new BaseResponseDto<int>(totalcount);
    }

    /// <summary>
    /// Gets the latest rental and returns a RentalDto
    /// </summary>
    /// <param name="httpContext">HttpContext containing user information</param>
    /// <returns>A BaseResponseDto containing the RentalDto object</returns>
    public async Task<BaseResponseDto<RentalDto>> GetLatestRental(HttpContext httpContext)
    {
        var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)!.Value;
        var rent = await (from r in _context.Rents
            join u in _context.Users on r.RequestedById equals u.Id
            where u.Email == userEmail
            orderby r.CreatedOn descending
            select new RentalDto().MapToDto(r)).FirstOrDefaultAsync();
        if (rent == null)
        {
            return new(true, "No rental found");
        }
        return new (rent);
    }
    
    /// <summary>
    /// Gets the car with the most rentals and returns a CarDto
    /// </summary>
    /// <returns>A BaseResponseDto containing the CarDto object</returns>
    public async Task<BaseResponseDto<CarDto>> GetMostRentedCar()
    {
        var mostRentedCar = await _context.Rents
            .GroupBy(r => r.CarId)
            .OrderBy(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        var carDetails = await _context.Cars
            .Where(c => c.Id == mostRentedCar)
            .Select(c => new CarDto().MapToDto(c))
            .FirstOrDefaultAsync();

        if (carDetails == null)
        {
            throw new DomainException("Car not found", 400);
        }

        return new BaseResponseDto<CarDto>(carDetails);
    }
    
    /// <summary>
    /// Gets the car with the least rentals and returns a CarDto
    /// </summary>
    /// <returns>A BaseResponseDto containing the CarDto object</returns>
    public async Task<BaseResponseDto<CarDto>> GetLeastRentedCar()
    {
        var leastRentedCar = await _context.Rents
            .GroupBy(r => r.CarId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        var carDetails = await _context.Cars
            .Where(c => c.Id == leastRentedCar)
            .Select(c => new CarDto().MapToDto(c))
            .FirstOrDefaultAsync();

        if (carDetails == null)
        {
            throw new DomainException("Car not found", 400);
        }

        return new BaseResponseDto<CarDto>(carDetails);
    }

}
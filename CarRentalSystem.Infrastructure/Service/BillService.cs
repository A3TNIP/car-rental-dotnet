using System.Security.Claims;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Service;

public class BillService: BaseService<Bill, BillDto>, IBillService
{
    private readonly ApplicationDbContext _context;
    public BillService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Get the latest bill for the logged in user
    /// </summary>
    /// <param name="context">HttpContext containing user information</param>
    /// <returns>BaseResponseDto containing the latest bill for the user</returns>
    public async Task<BaseResponseDto<BillDto>> GetLatestBill(HttpContext context)
    {
        // Get the user's email address from the JWT token
        var userEmail = context.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        // Get the user's id from the database
        var userId = await (from u in _context.Users where u.Email == userEmail select u.Id).FirstAsync();
        
        // Get the user's latest bill
        var latestBill = await (from b in _context.Bills
            join r in _context.Rents on b.RentalId equals r.Id
            join u in _context.Users on r.RequestedById equals u.Id
            join p in _context.Payments on b.Id equals p.BillId 
            where u.Id == userId
            && p.PaymentMethod != null
                orderby b.CreatedOn descending
                select b).FirstOrDefaultAsync();
        // If no bills are found, return an error
        if (latestBill == null)
        {
            return new BaseResponseDto<BillDto>(true, "No bills found");
        }
        // Return the bill
        return new BaseResponseDto<BillDto>(new BillDto());
    }

    /// <summary>
    /// Generate a new bill based on the provided data
    /// </summary>
    /// <param name="billDto">BillDto object containing bill data</param>
    /// <returns>BaseResponseDto containing the newly generated bill</returns>
    /// <exception cref="DomainException">Thrown when an error occurs during bill generation</exception>
    public async Task<BaseResponseDto<BillDto>> GenerateBill(BillDto billDto)
    {
        // Check if rental exists
        var rentId = billDto.RentalId;
        var rent = await _context.Rents.FindAsync(rentId);
        if (rent == null)
        {
            throw new DomainException("Rent not found", 404);
        }
        
        // Check if bill exists
        var eBill = await (from b in _context.Bills where b.RentalId == rentId select b).FirstOrDefaultAsync();
        if (eBill != null)
        {
            return new BaseResponseDto<BillDto>(new BillDto().MapToDto(eBill), "Bill already exists");
        }
        
        // Create new bill
        var nBill = billDto.MapToEntity();
        var damage = await (from d in _context.Damages where d.RentalId == rentId select d).FirstOrDefaultAsync();
        var carPrice = await (from c in _context.Cars where c.Id == rent.CarId select c.Rate).FirstAsync();
        var daysOfRent = rent.EndDate.Subtract(rent.StartDate).Days;
        nBill.TotalAmount += (carPrice * daysOfRent);
        nBill.Rate = carPrice;
        nBill.Discount = await (from o in _context.Offers where o.Id == rent.OfferId select o.Discount).FirstOrDefaultAsync();
        var userRole = await (from r in _context.Roles
            join ur in _context.UserRoles on r.Id equals ur.RoleId
            join u in _context.Users on ur.UserId equals u.Id 
            where u.Id == rent.RequestedById 
                select r.Name).FirstAsync();
        if (userRole == "Admin" || userRole == "Staff")
        {
            nBill.Discount += 10;
        }
        nBill.RepairCost = damage?.RepairCost ?? 0;
        nBill.TotalAmount = carPrice * daysOfRent * ((100 - nBill.Discount) / 100) + (damage?.RepairCost ?? 0);
        
        // Add bill to db
        var dbInstance = await _context.Set<Bill>().AddAsync(nBill);
        await _context.SaveChangesAsync();
        
        // Return bill
        var dto = new BillDto().MapToDto(dbInstance.Entity);
        return new BaseResponseDto<BillDto>(dto);
    }

    /// <summary>
    /// Get the bill with the provided rent id
    /// </summary>
    /// <param name="id">Rent id associated with the bill to be retrieved</param>
    /// <returns>BaseResponseDto containing the bill with the provided rent id</returns>
    /// <exception cref="DomainException">Thrown when the rent id is invalid or the bill cannot be found</exception>
    public async Task<BaseResponseDto<BillDto>> GetBillByRentId(string id)
    {
        var bill = await (from b in _context.Bills where b.RentalId == Guid.Parse(id) select new BillDto().MapToDto(b))
            .FirstOrDefaultAsync();
        
        // Check if bill is paid
        var payment = (from p in _context.Payments where p.BillId == bill.BillId select p).Count();
        if (payment > 0)
        {
            return new BaseResponseDto<BillDto>(true, "Bill already paid");
        }
        return new BaseResponseDto<BillDto>(bill);
    }

    /// <summary>
    /// Gets a list of sales for each car
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseDto<SalesDto>> GetSales()
    {
        var sales = await (from r in _context.Rents
            join c in _context.Cars on r.CarId equals c.Id
            where r.ApprovedById != null && r.EndDate < DateTime.UtcNow
            select c).ToListAsync();
        
        var salesD = await (from r in _context.Rents
            join c in _context.Cars on r.CarId equals c.Id
            where r.ApprovedById != null && r.EndDate < DateTime.UtcNow
            select r).ToListAsync();
        
        var groupedSales = sales.GroupBy(s => s).Select(s => new SalesDto()
        {
            carId = s.Key.Id.ToString(),
            carName = $"{s.Key.Brand} {s.Key.Model}",
            count = s.Count(),
            total = s.Key.Rate * s.Count(),
            rate = s.Key.Rate
        }).ToList();
        
        return new BaseResponseDto<SalesDto>(groupedSales);
    }

    public async Task<BaseResponseDto<SalesDto>> GetSalesFilteredList(SalesFilterDto dto)
    {
        var sales = (from r in _context.Rents
            join c in _context.Cars on r.CarId equals c.Id
            join u in _context.Users on r.RequestedById equals u.Id
            join au in _context.Users on r.ApprovedById equals au.Id 
            where r.ApprovedById != null && r.EndDate < DateTime.UtcNow 
                select new
                {
                    r,
                    c,
                    u,
                    au
                });
        if (dto.UserName != null)
        {
            sales = sales.Where(x => x.u.Name.ToLower().Contains(dto.UserName.ToLower()));
        }
        if (dto.StartDate != null)
        {
            sales = sales.Where(x => x.r.StartDate >= dto.StartDate);
        }
        if (dto.EndDate != null)
        {
            sales = sales.Where(x => x.r.EndDate <= dto.EndDate);
        }
        if (dto.AuthorizedBy != null)
        {
            sales = sales.Where(x => x.au.Name.ToLower().Contains(dto.AuthorizedBy.ToLower()));
        }

        var salesList = await sales.ToListAsync();
        var rDto = salesList.GroupBy(s => s.c).Select(s => new SalesDto()
        {
            carId = s.Key.Id.ToString(),
            carName = $"{s.Key.Brand} {s.Key.Model}",
            count = s.Count(),
            total = s.Key.Rate * s.Count(),
            rate = s.Key.Rate
        }).ToList();
        return new BaseResponseDto<SalesDto>(rDto);
    }
}
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Provider;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Service;

public class OfferService : BaseService<Offer, OfferDto>, IOfferService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfigService _configService;
    private readonly IEmailProvider _emailProvider;

    public OfferService(ApplicationDbContext context, IConfigService configService, IEmailProvider emailProvider) : base(context)
    {
        _context = context;
        _configService = configService;
        _emailProvider = emailProvider;
    }

    /// <summary>
    /// Retrieves the latest offers from the database.
    /// </summary>
    /// <param name="context">The HTTP context containing user information.</param>
    /// <returns>A task representing the asynchronous operation that returns the latest offers as a BaseResponseDto of OfferDto.</returns>
    public async Task<BaseResponseDto<OfferDto>> GetLatestOffersAsync(HttpContext context)
    {
        var userEmail = context.User.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;
        if (userEmail == null)
        {
            throw new DomainException("User not found", 400);
        }

        var user = _context.Users.FirstOrDefault(x => x.Email == userEmail) ??
                   throw new DomainException("User not found", 400);

        // Check if user is an active customer

        // Get total rents made by the user in the last 3 months
        var rents = await (from r in _context.Rents
            where r.RequestedById == user.Id
                  && r.CreatedOn >= DateTime.UtcNow.AddMonths(-3)
            select r).ToListAsync();
        var config = await _configService.GetByCodeAndKey(new ConfigDTO()
        {
            Code = "DB",
            Key = "MIN_OFFER_REQ_COUNT"
        });
        var value = Convert.ToInt32(config.Data.Value);
        var activeCustomer = rents.Count >= value;
        if (activeCustomer)
        {
            var data = await (from o in _context.Offers
                where o.ActiveStatus == true
                orderby o.EndDate descending
                select new OfferDto().MapToDto(o)).ToListAsync();
            return new BaseResponseDto<OfferDto>(data.GetRange(0, 2));
        }
        else
        {
            var data = await (from o in _context.Offers
                where o.ActiveStatus == true && o.Type == OfferType.Open
                orderby o.EndDate descending
                select new OfferDto().MapToDto(o)).ToListAsync();
            return new BaseResponseDto<OfferDto>(data.GetRange(0, 2));
        }
    }

    public async Task<BaseResponseDto<OfferDto>> CreateOfferAsync(OfferDto offerDto)
    {
        var dto = await Create(offerDto);

        List<User> recipients;
        if (dto.Data.Type == "Open")
        {
            recipients = await (from u in _context.Users
                join ur in _context.UserRoles on u.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Customer"
                select u).ToListAsync();
        }
        else
        {
            var config = await _configService.GetByCodeAndKey(new ConfigDTO()
            {
                Code = "DB",
                Key = "MIN_OFFER_REQ_COUNT"
            });
            var value = Convert.ToInt32(config.Data.Value);


            // Get total rents made by the user in the last 3 months
            var rents = await (from r in _context.Rents
            join u in _context.Users on r.RequestedById equals u.Id
                where r.CreatedOn >= DateTime.UtcNow.AddMonths(-3)
            select new
            {
                User = u,
                Rent = r
            }).ToListAsync();
            
            var groupedRents = rents.GroupBy(x => x.User)
                .Select(g => new
                {
                    User = g.Key,
                    Count = g.Count()
                })
                .ToList();
            recipients = groupedRents.Where(g => g.Count > value).Select(x => x.User).ToList();
        }


        var message = new EmailMessage
        {
            Subject = "New Offer",
            To = string.Join(",", recipients.Select(x => x.Email)),
            Body = @$"Dear Valued Customer,
We are delighted to inform you about our latest offer for you.

The offer details are as follows:

Start Date: {dto.Data.StartDate}
End Date: {dto.Data.EndDate}
Discount: {dto.Data.Discount}% off

Thank you for choosing our rental service. We hope you have a safe and enjoyable rides.

Best regards,
Hajurko Car Rental"
        };
        
        await _emailProvider.SendMultipleEmailAsync(message);

        return dto;
    }
}
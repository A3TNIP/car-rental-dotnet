using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

public class OffersController: BaseController<Offer, OfferDto>
{
    private readonly IOfferService _offerService;
    public OffersController(BaseService<Offer, OfferDto> baseService, IOfferService offerService) : base(baseService)
    {
        _offerService = offerService;
    }
    
    /// <summary>
    /// Gets the latest offers asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the latest offers.</returns>
    [HttpGet("LatestOffers")]
    public async Task<BaseResponseDto<OfferDto>> GetLatestOffersAsync()
    {
        return await _offerService.GetLatestOffersAsync(HttpContext);
    }
    
    [HttpPost("Create")]
    public async Task<BaseResponseDto<OfferDto>> CreateOffer(OfferDto offerDto)
    {
        return await _offerService.CreateOfferAsync(offerDto);
    }
}
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface IOfferService
{
    public Task<BaseResponseDto<OfferDto>> GetLatestOffersAsync(HttpContext context);
    public Task<BaseResponseDto<OfferDto>> CreateOfferAsync(OfferDto offerDto);
}
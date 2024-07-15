using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface IBillService
{
    public Task<BaseResponseDto<BillDto>> GetLatestBill(HttpContext context);
    public Task<BaseResponseDto<BillDto>> GenerateBill(BillDto billDto);
    public Task<BaseResponseDto<BillDto>> GetBillByRentId(string id);
    public Task<BaseResponseDto<SalesDto>> GetSales();
    Task<BaseResponseDto<SalesDto>> GetSalesFilteredList(SalesFilterDto dto);
}
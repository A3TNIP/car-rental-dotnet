using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Base;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Common;

[ApiController]
[Route(ApiConstants.TEMPLATE)]
public abstract class BaseController<TE, TD>: ControllerBase where TE : BaseEntity where TD : IBaseDto<TE, TD>, new()
{
    private readonly BaseService<TE, TD> _baseService;
    
    protected BaseController(BaseService<TE, TD> baseService)
    {
        _baseService = baseService;
    }
    
    [HttpGet]
    public async Task<BaseResponseDto<TD>> GetAll()
    {
        return await _baseService.GetAll();
    }
    
    [HttpGet("{id}")]
    public async Task<BaseResponseDto<TD>> GetById(Guid id)
    {
        return await _baseService.GetById(id);
    }
    
    [HttpPost]
    public async Task<BaseResponseDto<TD>> Create(TD dto)
    {
        return await _baseService.Create(dto);
    }
    
    [HttpPut]
    public async Task<BaseResponseDto<TD>> Update(TD dto)
    {
        return await _baseService.Update(dto);
    }
    
    [HttpDelete("{id}")]
    public async Task<BaseResponseDto<TD>> Delete(Guid id)
    {
        return await _baseService.Delete(id);
    }
}
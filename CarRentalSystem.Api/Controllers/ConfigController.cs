using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
public class ConfigController: BaseController<Config, ConfigDTO>
{

    private readonly IConfigService _configService;

    public ConfigController(BaseService<Config, ConfigDTO> baseService, IConfigService configService) : base(baseService)
    {
        _configService = configService;
    }

    /// <summary>
    /// Retrieves a configuration value by its code and key.
    /// </summary>
    /// <param name="dto">The configuration DTO with code and key.</param>
    /// <returns>A response DTO containing the requested configuration value.</returns>
    [HttpGet("GetByCodeAndKey")]
    public async Task<BaseResponseDto<ConfigDTO>> GetByCodeAndKey([FromBody] ConfigDTO dto)
    {
        return await _configService.GetByCodeAndKey(dto);
    }
}
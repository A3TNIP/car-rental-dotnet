using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Service;

public class ConfigService: BaseService<Config, ConfigDTO>, IConfigService
{
    private readonly ApplicationDbContext _context;

    public ConfigService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Get configuration by code and key.
    /// </summary>
    /// <param name="dto">ConfigDTO object containing the code and key.</param>
    /// <returns>BaseResponseDto containing the ConfigDTO object.</returns>
    public async Task<BaseResponseDto<ConfigDTO>> GetByCodeAndKey(ConfigDTO dto)
    {
        // get config by code and key
        var config = await (from c in _context.Configs
            where c.Code == dto.Code && c.Key == dto.Key
            select new ConfigDTO
            {
                Id = c.Id,
                Code = c.Code,
                Key = c.Key,
                Value = c.Value
            }).FirstOrDefaultAsync() ?? throw new DomainException("Config not found", 404);

        // return config
        return new BaseResponseDto<ConfigDTO>(config);
    }
}
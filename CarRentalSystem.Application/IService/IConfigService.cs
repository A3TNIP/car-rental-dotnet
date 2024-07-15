using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.Base;

namespace CarRentalSystem.Application.IService;

public interface IConfigService
{
    public Task<BaseResponseDto<ConfigDTO>> GetByCodeAndKey(ConfigDTO dto);
}

using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;
using File = CarRentalSystem.Domain.Entities.File;

namespace CarRentalSystem.Application.IService;

public interface IAttachmentService
{
    public Task<BaseResponseDto<AttachmentDto>> Upload(IFormFile file, string userEmail, HttpContext context);
    public Task<File> GetFile(Guid id);
}
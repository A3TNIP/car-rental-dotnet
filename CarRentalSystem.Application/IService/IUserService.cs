using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface IUserService
{
    public Task<BaseResponseDto<UserDTO>> GetAllStaff();
    public Task<BaseResponseDto<UserDTO>> GetAllAdmin();
    public Task<BaseResponseDto<UserDTO>> GetAllCustomer();
    public Task<BaseResponseDto<UserDTO>> GetAll();
    public Task<BaseResponseDto<UserDTO>> UpdateUser(UserDTO dto);
    public Task<BaseResponseDto<UserDTO>> DeleteUser(string id);
    public Task<BaseResponseDto<UserDTO>> UploadDocument(HttpContext context, IFormFile file, string documentType);

    public Task<BaseResponseDto<int>> GetStaffCount();
    Task<BaseResponseDto<UserDTO>> GetDto(HttpContext httpContext);
    public Task<BaseResponseDto<int>> GetRegularCustomerCount();
    public Task<BaseResponseDto<int>> GetAllCustomerCount();
    
}
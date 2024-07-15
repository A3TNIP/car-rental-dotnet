using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Application.IService;

public interface IAuthenticationService
{
    public Task<AuthenticationResponse> Login(LoginRequest loginRequest);
    public Task<AuthenticationResponse> Register(UserDTO userDto);
    public Task<UserPrincipal> GetPrincipal(HttpContext httpContext);
    public Task<BaseResponseDto<UserDTO>> ChangePassword(string userId, ChangePasswordRequest changePasswordRequest);
}
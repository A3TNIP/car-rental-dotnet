using System.Security.Claims;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

[ApiController]
[Route("Api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Endpoint for user login
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    public async Task<AuthenticationResponse> Login(LoginRequest loginRequest)
    {
        return await _authenticationService.Login(loginRequest);
    }

    /// <summary>
    /// Endpoint for user registration
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns></returns>
    [HttpPost("Register")]
    public async Task<AuthenticationResponse> Register([FromBody] UserDTO userDto)
    {
        return await _authenticationService.Register(userDto);
    }
    
    /// <summary>
    /// Endpoint for retrieving user principal
    /// </summary>
    /// <returns></returns>
    [HttpGet("Principal")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<UserPrincipal> GetPrincipal()
    {
        return await _authenticationService.GetPrincipal(HttpContext);

    }

    /// <summary>
    /// Endpoint for changing user password
    /// </summary>
    /// <param name="changePasswordRequest"></param>
    /// <returns></returns>
    [HttpPost("Change-Password")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var userId = User.FindFirst(ClaimTypes.Email)?.Value;
        await _authenticationService.ChangePassword(userId!, changePasswordRequest);
    }
}
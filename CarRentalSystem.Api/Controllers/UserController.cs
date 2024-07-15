using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Infrastructure.Service;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

[ApiController]
[Route(ApiConstants.TEMPLATE)]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// <summary>
    /// Get all staff members
    /// </summary>
    [HttpGet("Staff")]
    public async Task<BaseResponseDto<UserDTO>> GetAllStaff()
    {
        return await _userService.GetAllStaff();
    }
    
    /// <summary>
    /// Get all admin users
    /// </summary>
    [HttpGet("Admin")]
    public async Task<BaseResponseDto<UserDTO>> GetAllAdmin()
    {
        return await _userService.GetAllAdmin();
    }
    
    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet("Customer")]
    public async Task<BaseResponseDto<UserDTO>> GetAllCustomer()
    {
        return await _userService.GetAllCustomer();
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<BaseResponseDto<UserDTO>> GetAll()
    {
        return await _userService.GetAll();
    }

    /// <summary>
    /// Update user information
    /// </summary>
    /// /// <param name="dto"></param>
    [HttpPatch]
    public async Task<BaseResponseDto<UserDTO>> Update([FromBody] UserDTO dto)
    {
        return await _userService.UpdateUser(dto);
    }
    
    /// <summary>
    /// Delete a user by id
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}")]
    public async Task<BaseResponseDto<UserDTO>> Delete(string id)
    {
        return await _userService.DeleteUser(id);
    }
    
    /// <summary>
    /// Upload a document for a user
    /// </summary>
    /// <param name="file"></param>
    /// <param name="documentType"></param>
    [HttpPost("UploadDocument")]
    public async Task<BaseResponseDto<UserDTO>> UploadDocument([FromForm] IFormFile file, [FromForm] string documentType)
    {
        return await _userService.UploadDocument(HttpContext, file, documentType);
    }
    
    /// <summary>
    /// Get the user DTO for the current user
    /// </summary>
    [HttpGet("GetDto")]
    public async Task<BaseResponseDto<UserDTO>> GetDto()
    {
        return await _userService.GetDto(HttpContext);
    }

    /// <summary>
    /// Get the total number of staff members
    /// </summary>
    [HttpGet("GetStaffCount")]
    public async Task<BaseResponseDto<int>> GetStaffCount()
    {
        return await _userService.GetStaffCount();
    }
    
    /// <summary>
    /// Get the total number of customers
    /// </summary>
    [HttpGet("GetAllCustomerCount")]
    public async Task<BaseResponseDto<int>> GetAllCustomerCount()
    {
        return await _userService.GetAllCustomerCount();
    }

    /// <summary>
    /// Get the number of regular customers
    /// </summary>
    [HttpGet("GetRegularCustomerCount")]
    public async Task<BaseResponseDto<int>> GetRegularCustomerCount()
    { 
        
        return await _userService.GetRegularCustomerCount();
    }
}
  
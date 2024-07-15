using System.Security.Claims;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Base;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Service;

public class UserService: IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IAttachmentService _attachmentService;
    private readonly ApplicationDbContext _context;
    public UserService(ApplicationDbContext context, UserManager<User> userManager, IAttachmentService attachmentService)
    {
        _userManager = userManager;
        _attachmentService = attachmentService;
        _context = context;
    }
    
    /// <summary>
    /// This method returns all the staff members from the database
    /// </summary>
    /// <returns>A BaseResponseDto containing a list of UserDTO objects</returns>
    public async Task<BaseResponseDto<UserDTO>> GetAllStaff()
    {
        // Get users who have staff role
        var data = (from dbUsers in _context.Users
            join dbUserRoles in _context.UserRoles
                on dbUsers.Id equals dbUserRoles.UserId
            join dbRoles in _context.Roles on dbUserRoles.RoleId equals dbRoles.Id
            where dbUsers.ActiveStatus == ActiveStatus.Active && dbRoles.Name == "Staff"
            select new UserDTO().MapToDto(dbUsers)).ToList();
        return new BaseResponseDto<UserDTO>(data);
    }

    /// <summary>
    /// This method returns all the admin users from the database
    /// </summary>
    /// <returns>A BaseResponseDto containing a list of UserDTO objects</returns>
    public async Task<BaseResponseDto<UserDTO>> GetAllAdmin()
    {
        var users = await _userManager.GetUsersInRoleAsync("Admin");
        var usersDto = users.Where(u => u.ActiveStatus == ActiveStatus.Active).Select(entity => new UserDTO().MapToDto(entity)).ToList();
        return new BaseResponseDto<UserDTO>(usersDto);
    }

    /// <summary>
    /// This method returns all the customer users from the database along with their documents and active status
    /// </summary>
    /// <returns>A BaseResponseDto containing a list of UserDTO objects</returns>
    public async Task<BaseResponseDto<UserDTO>> GetAllCustomer()
    {
        var data = await (from users in _context.Users
            join userRoles in _context.UserRoles on users.Id equals userRoles.UserId
            join roles in _context.Roles on userRoles.RoleId equals roles.Id
            where users.ActiveStatus == ActiveStatus.Active && roles.Name == "Customer"
            select new
            {
                User = users,
                LatestRent = _context.Rents
                    .Where(r => r.RequestedById == users.Id)
                    .OrderByDescending(r => r.CreatedOn)
                    .Select(r => r.CreatedOn)
                    .FirstOrDefault()
            }).ToListAsync();

        var currentDate = DateTime.UtcNow;

        var customers = data.Select(x =>
        {
            var dto = new UserDTO().MapToDto(x.User);
            var document = (from d in _context.Documents where d.Id == x.User.DocumentId select d).FirstOrDefault();
            dto.Document = document;
            dto.IsActive = (x.LatestRent != DateTime.MinValue && (currentDate - x.LatestRent).TotalDays <= 90);
            return dto;
        }).ToList();
        return new BaseResponseDto<UserDTO>(customers);
    }
    
    /// <summary>
    /// This method updates a user's information in the database
    /// </summary>
    /// <param name="dto">A UserDTO object containing the user's updated information</param>
    /// <returns>A BaseResponseDto containing a UserDTO object with the updated information</returns>
    /// <exception cref="DomainException">Thrown when user is not found or is already deleted</exception>
    public async Task<BaseResponseDto<UserDTO>> UpdateUser(UserDTO dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id!);
        if (user == null || user.ActiveStatus == ActiveStatus.Deleted)
        {
            throw new DomainException($"User not found with id {dto.Id}", 400);
        }
        user.Address = dto.Address;
        user.PhoneNumber = dto.PhoneNumber;
        user.Name = dto.Name;
        
        var dbInstance = await _userManager.UpdateAsync(user);
        if (!dbInstance.Succeeded)
        {
            throw new DomainException("Failed to update user", 400);
        }
        return new BaseResponseDto<UserDTO>(true, "User updated successfully");
    }

    /// <summary>
    /// This method soft deletes a user from the database
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>A BaseResponseDto indicating whether the operation was successful or not</returns>
    /// <exception cref="DomainException">Thrown when user is not found or is already deleted</exception>
    public async Task<BaseResponseDto<UserDTO>> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || user.ActiveStatus == ActiveStatus.Deleted)
        {
            throw new DomainException("User not found", 400);
        }

        user.ActiveStatus = ActiveStatus.Deleted;
        var dbInstance = await _userManager.UpdateAsync(user);
        if (!dbInstance.Succeeded)
        {
            throw new DomainException("Failed to delete user", 400);
        }
        return new BaseResponseDto<UserDTO>(true, "User deleted successfully");
    }

    /// <summary>
    /// This method uploads a document for a user in the database
    /// </summary>
    /// <param name="context">The current HTTP context</param>
    /// <param name="file">The file to upload</param>
    /// <param name="documentType">The type of document to upload</param>
    /// <returns>A BaseResponseDto indicating whether the operation was successful or not</returns>
    /// <exception cref="DomainException">Thrown when the user or document is not found, or the document type is invalid</exception>
    public async Task<BaseResponseDto<UserDTO>> UploadDocument(HttpContext context, IFormFile file, string documentType)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.Email)!.Value;
        var attachment = await _attachmentService.Upload(file, userIdClaim, context);
        var user = await _userManager.FindByEmailAsync(userIdClaim);
        Document document = new()
        {
            FileURL = attachment.Data.FileUrl
        };
        document.Type = documentType.ToUpper() switch
        {
            "LICENSE" => DocumentType.License,
            "CITIZENSHIP" => DocumentType.Citizenship,
            _ => throw new DomainException("Invalid document type", 400)
        };
        var dbInstance = await _context.Set<Document>().AddAsync(document);
        await _context.SaveChangesAsync();
        user.Document = dbInstance.Entity;
        var userDbInstance = await _userManager.UpdateAsync(user);
        if (!userDbInstance.Succeeded)
        {
            throw new DomainException("Failed to upload document", 400);
        }
        return new BaseResponseDto<UserDTO>(true, "Document uploaded successfully");
    }

    
    public async Task<BaseResponseDto<UserDTO>> GetAll()
    {
        var dbInstance = await _userManager.Users.Where(u => u.ActiveStatus == ActiveStatus.Active).ToListAsync();
        var usersDto = dbInstance.Select(entity => new UserDTO().MapToDto(entity)).ToList();
        return new BaseResponseDto<UserDTO>(usersDto);
    }
    
    /// <summary>
    /// This method gets the count of all staff members in the database
    /// </summary>
    /// <returns>A BaseResponseDto containing an integer value of the count</returns>
    public async Task<BaseResponseDto<int>> GetStaffCount()
    {
        var staffCount =  await (from dbUsers in _context.Users
            join dbUserRoles in _context.UserRoles
                on dbUsers.Id equals dbUserRoles.UserId
            join dbRoles in _context.Roles on dbUserRoles.RoleId equals dbRoles.Id
            where dbUsers.ActiveStatus == ActiveStatus.Active && dbRoles.Name == "Staff"
            select dbUsers).CountAsync();
        return new BaseResponseDto<int>(staffCount);
    }

    /// <summary>
    /// This method gets the currently logged-in user's information from the database
    /// </summary>
    /// <param name="httpContext">The current HTTP context</param>
    /// <returns>A BaseResponseDto containing the current user's UserDTO object</returns>
    public async Task<BaseResponseDto<UserDTO>> GetDto(HttpContext httpContext)
    {
        var email = httpContext.User.FindFirst(ClaimTypes.Email)!.Value;
        var user = await (from u in _context.Users where u.Email == email select u).FirstAsync();
        var document = (from d in _context.Documents where d.Id == user.DocumentId select d).FirstOrDefault();
        
        var userDto = new UserDTO().MapToDto(user);
        userDto.Document = document;
        return new BaseResponseDto<UserDTO>(userDto);
    }
    
    /// <summary>
    /// This method gets the count of all customer users in the database
    /// </summary>
    /// <returns>A BaseResponseDto containing an integer value of the count</returns>
    public async Task<BaseResponseDto<int>> GetAllCustomerCount()
    {
        var allcustomer = await (from users in _context.Users
            join userRoles in _context.UserRoles on users.Id equals userRoles.UserId
            join roles in _context.Roles on userRoles.RoleId equals roles.Id
            where users.ActiveStatus == ActiveStatus.Active && roles.Name == "Customer"
            select users).CountAsync();
        return new BaseResponseDto<int>(allcustomer);
    }
    
    /// <summary>
    /// This method gets the count of regular customer users in the database
    /// </summary>
    /// <returns>A BaseResponseDto containing an integer value of the count</returns>
    public async Task<BaseResponseDto<int>> GetRegularCustomerCount()
    {
        var data = await (from users in _context.Users
            join userRoles in _context.UserRoles on users.Id equals userRoles.UserId
            join roles in _context.Roles on userRoles.RoleId equals roles.Id
            where users.ActiveStatus == ActiveStatus.Active && roles.Name == "Customer"
            select new
            {
                User = users,
                LatestRent = _context.Rents
                    .Where(r => r.RequestedById == users.Id)
                    .OrderByDescending(r => r.CreatedOn)
                    .Select(r => r.CreatedOn)
                    .FirstOrDefault()
            }).ToListAsync();

        var currentDate = DateTime.UtcNow;

        var customers = data.Select(x =>
        {
            var dto = new UserDTO().MapToDto(x.User);
            dto.IsActive = (x.LatestRent != DateTime.MinValue && (currentDate - x.LatestRent).TotalDays <= 90);
            return dto;
        }).Count(dto => dto.IsActive ?? false);
        return new BaseResponseDto<int>(customers);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace CarRentalSystem.Infrastructure.Service;

public class AuthenticationService: IAuthenticationService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthenticationService(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
        _context = context;
    }

    /// <summary>
    /// Authenticate a user
    /// </summary>
    /// <param name="loginRequest">The login request DTO</param>
    /// <returns>An authentication response DTO</returns>
    public async Task<AuthenticationResponse> Login(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            // user does not exist
            throw new DomainException("Invalid Login Attempt", 400);
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        if (result.Succeeded)
        {
            // password is correct
            var token = GenerateJwtToken(user);
            return new AuthenticationResponse(token);
        }
        // password is incorrect
        throw new DomainException("Invalid Login Attempt", 400);
    }
    
    /// <summary>
    /// Takes a user and generates a JWT token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private string GenerateJwtToken(User user)
    {
        // create the key used to sign the token
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);
        // get the user's role
        var role = _userManager.GetRolesAsync(user).Result.First();
        // create the token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // create the claims for the user
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            // the token will expire in 1 hour
            Expires = DateTime.UtcNow.AddHours(1),
            // sign the token with the secret key
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // create the token handler
        var tokenHandler = new JwtSecurityTokenHandler();
        // create the token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        // return the token
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="userDto">The user DTO containing registration information</param>
    /// <returns>An authentication response DTO</returns>
    public async Task<AuthenticationResponse> Register(UserDTO userDto)
    {
        // Get User from DTO
        var user = new User
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            Name = userDto.Name,
            PhoneNumber = userDto.PhoneNumber,
            Address = userDto.Address,
            ActiveStatus = ActiveStatus.Active
        };

        // Save to Database
        var dbInstance = await _userManager.CreateAsync(user, userDto.Password);

        // If successful, add role and generate token
        if (dbInstance.Succeeded)
        {
            // Add Role
            var role = userDto.Role ?? Roles.Customer;
            var roleName = role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);

            // Return token
            var token = GenerateJwtToken(user);
            return new AuthenticationResponse(token);
        }

        throw new DomainException("User registration failed", 400);
    }

    /// <summary>
    /// Get the current user principal from the given HttpContext
    /// </summary>
    /// <param name="httpContext">The HttpContext to extract the user from</param>
    /// <returns>The current user principal</returns>
    public async Task<UserPrincipal> GetPrincipal(HttpContext context)
    {
    // Get user based on email
    var user = await _userManager.FindByEmailAsync(context.User.FindFirst(ClaimTypes.Email)!.Value);
    // Instantiate a new UserPrincipal based on user
    UserPrincipal principal = new UserPrincipal(user);
    // Get role from user manager
    var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault()!;
    principal.RoleName = role;
    principal.RoleEnum = RoleHelper.GetRoleFromString(role);
    // Get document based on user
    var document = await (from d in _context.Documents join u in _context.Users on d.Id equals u.DocumentId 
                where u.Id == user.Id select d).FirstOrDefaultAsync();
    // Set has document property
    principal.HasDocument = document != null;
    // Get all bills based on user
    var bills = await (from b in _context.Bills
        join r in _context.Rents on b.RentalId equals r.Id 
        where r.RequestedById == user.Id select b).ToListAsync();
    // Get all payments based on user
    var payments = await (from p in _context.Payments where p.CustomerId == user.Id select p).ToListAsync();
    // Check if there is all bill has been paid by checking if bill id is in payment
    principal.HasNoBills = bills.All(b => payments.Any(p => p.BillId == b.Id));
    // Return principal
    return principal;
    }
    
    /// <summary>
    /// Change the password for a given user
    /// </summary>
    /// <param name="userId">The ID of the user whose password should be changed</param>
    /// <param name="changePasswordRequest">The change password request DTO containing the old and new passwords</param>
    /// <returns>A response DTO indicating success or failure of the operation</returns>
    public async Task<BaseResponseDto<UserDTO>> ChangePassword(string userId, ChangePasswordRequest changePasswordRequest)
    {
        // Find the user
        var user = await _userManager.FindByEmailAsync(userId);
        if (user == null)
        {
            throw new DomainException("User not found", 404);
        }

        // Change the password
        var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
        if (!result.Succeeded)
        {
            throw new DomainException($"Error changing password!", 400);
        }

        // Return the result
        return new BaseResponseDto<UserDTO>(true, "Password changed successfully!");
    }
}
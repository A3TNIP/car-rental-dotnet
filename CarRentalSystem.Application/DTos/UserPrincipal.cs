using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Application.DTos;

public class UserPrincipal
{
    public string Email { get; set; }
    public string Name { get; set; }
    public Roles RoleEnum { get; set; }
    public string RoleName { get; set; }
    public bool HasDocument { get; set; }
    public bool HasNoBills { get; set; }

    public UserPrincipal(User user)
    {
        Email = user.Email;
        Name = user.Name;
    }
}
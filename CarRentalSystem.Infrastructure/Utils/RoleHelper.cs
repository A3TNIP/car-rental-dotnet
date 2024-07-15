using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Infrastructure.Utils;

public static class RoleHelper
{
    public static Roles GetRoleFromString(string roleString)
    {
        if (string.IsNullOrEmpty(roleString))
        {
            throw new ArgumentNullException(nameof(roleString));
        }

        switch (roleString.ToUpper())
        {
            case "ADMIN":
                return Roles.Admin;
            case "STAFF":
                return Roles.Staff;
            case "CUSTOMER":
                return Roles.Customer;
            default:
                throw new ArgumentException($"Invalid role string: {roleString}", nameof(roleString));
        }
    }
}
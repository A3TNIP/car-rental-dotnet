using System.ComponentModel.DataAnnotations;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Application.DTos;

public class UserDTO: IBaseDto<User, UserDTO>
{
    public string? Id { get; set; }
    public String Name { get; set; }
    public String Email { get; set; }
    public String PhoneNumber { get; set; }
    public String Address { get; set; }
    public String? Password { get; set; }
    public Document? Document { get; set; }
    public bool? IsActive { get; set; }
    public Roles? Role { get; set; }
    public UserDTO MapToDto(User entity)
    {
        return new UserDTO()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            Address = entity.Address,
            Document = entity.Document
        };
    }

    public User MapToEntity()
    {
        return new User
        {
            UserName = Email,
            Email = Email,
            Name = Name,
            PhoneNumber = PhoneNumber,
            Address = Address,
        };
    }

    public User UpdateEntity(User entity, UserDTO dto)
    {
        entity.Name = dto.Name;
        entity.Email = dto.Email;
        entity.PhoneNumber = dto.PhoneNumber;
        entity.Address = dto.Address;
        return entity;
    }

    public Guid GetId()
    {
        if (Id == null)
        {
            throw new InvalidOperationException("Id cannot be null.");
        }

        return Id == null ? Guid.Empty : Guid.Parse(Id);
    }

    public string GetName()
    {
        return Email;
    }
}
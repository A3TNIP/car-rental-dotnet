using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarRentalSystem.Domain.Base;
using CarRentalSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CarRentalSystem.Domain.Entities;

public class User: IdentityUser
{
    [Required(ErrorMessage = "Name is required.")]
    public String Name { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    public String PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }
    
    [ForeignKey("Document")]
    public Guid? DocumentId { get; set; }
    public Document? Document { get; set; }

    public ActiveStatus ActiveStatus { get; set; }

}
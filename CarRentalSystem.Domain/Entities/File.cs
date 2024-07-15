using System.ComponentModel.DataAnnotations.Schema;
using CarRentalSystem.Domain.Base;

namespace CarRentalSystem.Domain.Entities;

public class File: BaseEntity
{
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
    public string Metadata { get; set; }
    
    [ForeignKey("UploadedBy")]
    public string UploadedById { get; set; }
    public User UploadedBy { get; set; }
}
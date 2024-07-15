using System.ComponentModel.DataAnnotations;
using CarRentalSystem.Domain.Enums;

namespace CarRentalSystem.Domain.Entities;


public class Document
{
    public Guid Id { get; private set; }
    public DocumentType Type { get; set; }
    public string FileURL { get; set; }
    
    public Document()
    {
        Id = Guid.NewGuid();
    }
}
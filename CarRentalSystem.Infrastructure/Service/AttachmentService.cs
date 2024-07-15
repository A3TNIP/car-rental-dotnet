using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Exceptions;
using CarRentalSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using File = CarRentalSystem.Domain.Entities.File;

namespace CarRentalSystem.Infrastructure.Service;

public class AttachmentService : IAttachmentService
{
    private readonly ApplicationDbContext _dbContext;

    public AttachmentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Uploads a file to the database
    /// Saves the user's email who uploaded the file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="userEmail"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    public async Task<BaseResponseDto<AttachmentDto>> Upload(IFormFile file, string userEmail, HttpContext context)
    {
        // Get File name
        var fileName = file.FileName;

        // Check if file was uploaded
        if (file == null)
        {
            throw new DomainException("No file was uploaded", 400);
        }

        // Check if invalid file name
        if (fileName.Contains(".."))
        {
            throw new DomainException("File name contains invalid characters", 400);
        }
        
        // Check size of file (max file size is 1.5 MB)
        if (file.Length > (1.5 * 1024 * 1024))
        {
            throw new DomainException("File size is too large", 400);
        }

        // Only accept pdf and png files
        if (file.ContentType != "application/pdf" && file.ContentType != "image/png")
        {
            throw new DomainException("File type is not supported", 400);
        }

        // Convert file to byte array
        await using var memorySteam = new MemoryStream();
        await file.CopyToAsync(memorySteam);
        var fileContent = memorySteam.ToArray();

        // Find user who uploaded file
        var user = _dbContext.Set<User>().First(x => x.Email == userEmail);

        // Create file model
        var fileModel = new File
        {
            FileName = fileName,
            FileContent = fileContent,
            Metadata = file.ContentType,
            UploadedById = user.Id
        };

        // Save file to database
        EntityEntry<File> dbInstance = await _dbContext.Set<File>().AddAsync(fileModel);
        
        // Commit changes to database
        await _dbContext.SaveChangesAsync();

        // Get file url
        var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
        var fileUrl = $"{baseUrl}/Api/Attachments/{dbInstance.Entity.Id}";

        // Create attachment dto
        AttachmentDto attachmentDto = new()
        {
            UploadedBy = userEmail,
            FileName = fileName,
            FileExtension = file.Headers.ContentType!,
            FileUrl = fileUrl
        };

        // Return attachment dto
        return new BaseResponseDto<AttachmentDto>(attachmentDto);
    }

    /// <summary>
    /// Get file from database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    public async Task<File> GetFile(Guid id)
    {
        // Find file in database
        return await _dbContext.Set<File>().FindAsync(id) ?? throw new DomainException("File not found", 404);
    }
}
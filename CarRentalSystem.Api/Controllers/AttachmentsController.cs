using System.Security.Claims;
using CarRentalSystem.Api.Common;
using CarRentalSystem.Application.Base;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Api.Controllers;

[ApiController]
[Route(ApiConstants.TEMPLATE)]
public class AttachmentsController: ControllerBase
{
    
    private readonly IAttachmentService _attachmentService;

    public AttachmentsController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    /// <summary>
    /// API endpoint for uploading attachments
    /// </summary>
    /// <param name="file">The attachment file to upload</param>
    /// <returns>A response containing the uploaded attachment's metadata</returns>
    /// <exception cref="DomainException">Thrown when the user is not authenticated</exception>
    [HttpPost(ApiConstants.UPLOAD)]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<BaseResponseDto<AttachmentDto>> Upload([FromForm] IFormFile file)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
        if (userIdClaim == null)
        {
            throw new DomainException("User is not authenticated", 401);
        }

        var userEmail = userIdClaim;
        return await _attachmentService.Upload(file, userEmail, HttpContext);
    }
    
    /// <summary>
    /// API endpoint for getting an attachment file by its ID
    /// </summary>
    /// <param name="id">The ID of the attachment file to retrieve</param>
    /// <param name="download">Indicates whether to force the file to be downloaded instead of displayed in the browser</param>
    /// <returns>A file stream containing the attachment file</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(Guid id, [FromQuery] bool download = false)
    {
        var file = await _attachmentService.GetFile(id);

        var fileStream = new MemoryStream(file.FileContent);
        var contentType = file.Metadata;
        var fileName = file.FileName;

        if (download)
        {
            return File(fileStream, contentType, fileName);
        }

        return File(fileStream, contentType);
    }
}
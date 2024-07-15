using CarRentalSystem.Application.DTos;

namespace CarRentalSystem.Application.IService;

public interface IEmailProvider
{
    public Task SendEmailAsync(EmailMessage message);
    public Task SendMultipleEmailAsync(EmailMessage message);
}
using System.Net;
using System.Net.Mail;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using Microsoft.Extensions.Configuration;

namespace CarRentalSystem.Infrastructure.Provider;

public class GmailEmailProvider: IEmailProvider
{
    private readonly string _from;
    private readonly SmtpClient _client; 
    
    public GmailEmailProvider(IConfiguration configuration) { 
        var userName = configuration.GetSection("GmailCredentials:UserName").Value!; 
        var password = configuration.GetSection("GmailCredentials:Password").Value!; 
        _from = userName; 
        _client = new SmtpClient("smtp.gmail.com", 587) { 
            Credentials = new NetworkCredential(userName, password), 
            UseDefaultCredentials = false, 
            EnableSsl = true 
        }; 
    }
    
    public async Task SendEmailAsync(EmailMessage message) 
    {
        var mailMessage = new MailMessage(_from, message.To, message.Subject, message.Body);
        await _client.SendMailAsync(mailMessage); 
    }

    public async Task SendMultipleEmailAsync(EmailMessage message)
    {
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_from);
        mailMessage.Subject = message.Subject;
        mailMessage.Body = message.Body;
        var recipients = message.To.Split(',');
        foreach (var r in recipients)
        {
            mailMessage.Bcc.Add(r.Trim());
        }

        await _client.SendMailAsync(mailMessage);
    }
}
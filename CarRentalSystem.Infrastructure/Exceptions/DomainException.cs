namespace CarRentalSystem.Infrastructure.Exceptions;

public class DomainException: Exception
{
    public int StatusCode { get; set; }
    
    public DomainException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
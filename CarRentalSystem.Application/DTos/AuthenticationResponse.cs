namespace CarRentalSystem.Application.DTos;

public class AuthenticationResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string? Token { get; set; }

    public AuthenticationResponse(string token)
    {
        IsSuccess = true;
        Message = "Token Generated Successfully";
        this.Token = token;
    }

    public AuthenticationResponse()
    {
        IsSuccess = false;
        Message = "Invalid Login Attempt";
    }
}
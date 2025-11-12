namespace ProductCatalog.Application.DTOs;

public class AuthReponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public UserDto User { get; set; }
}
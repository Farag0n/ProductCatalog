namespace ProductCatalog.Application.Interfaces;

public interface IAuthService
{
    Task<string?> Authenticate(string userName, string password);
}
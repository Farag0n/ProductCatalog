using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Interfaces;

public interface IAuthService
{
    Task<AuthReponseDto> RefreshTokenAsync(RefreshTokenRequestDto requestDto);
    Task LogoutAsync();
}
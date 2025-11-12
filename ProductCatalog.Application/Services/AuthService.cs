using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using System.Collections.Concurrent;

namespace ProductCatalog.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _config;
    private static readonly ConcurrentDictionary<string, (string Email, DateTime Expires)> _refreshTokens = new();

    public AuthService(IUserRepository repository, IConfiguration config)
    {
        _repository = repository;
        _config = config;
    }
    
    public async Task<AuthReponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        if (!_refreshTokens.TryGetValue(request.RefreshToken, out var tokenData))
            throw new UnauthorizedAccessException("Refresh token inválido.");

        if (tokenData.Expires < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expirado.");

        var users = await _repository.GetAllUser();
        var existingUser = users.FirstOrDefault(u => u.Email == tokenData.Email);
        if (existingUser == null)
            throw new UnauthorizedAccessException("Usuario no encontrado.");

        var newAccessToken = GenerateJwtToken(existingUser);
        var newRefreshToken = GenerateRefreshToken();
        SaveRefreshToken(newRefreshToken, existingUser.Email);

        return new AuthReponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            User = new UserDto
            {
                Name = existingUser.Name,
                LastName = existingUser.LastName,
                Email = existingUser.Email,
                UserName = existingUser.UserName,
                Role = existingUser.Role.ToString()
            }
        };
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }
    
    //Metodos privados
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("id", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private void SaveRefreshToken(string token, string email)
    {
        _refreshTokens[token] = (email, DateTime.UtcNow.AddDays(7));
    }
}
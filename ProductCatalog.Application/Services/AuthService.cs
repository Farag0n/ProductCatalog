using Microsoft.Extensions.Configuration;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProductCatalog.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public async Task<string?> Authenticate(string userName, string password)
    {
        //Obtener usuario por userName
        var users = await _userRepository.GetAllUser();
        var user = users.FirstOrDefault(u => u.UserName == userName);

        if (user == null)
            return null;
        
        //validar pasword hasheada
        bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!passwordValid)
            return null;
        
        //generar token claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        //Crear Token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: cred
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
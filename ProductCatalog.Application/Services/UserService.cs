using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }
    
    private UserDto MapUserToDto(User user)
    {
        return new UserDto
        {
            Name = user.Name,
            LasName = user.LasName,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role.ToString() // Convertir enum a string
        };
    }

    private string GenerateJwtToken(User user)
    {
        // Leer la configuración desde appsettings.json
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] 
                                                                  ?? throw new InvalidOperationException("Configuración JWT 'Key' no encontrada")));
            
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Configuración JWT 'Issuer' no encontrada");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Configuración JWT 'Audience' no encontrada");

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Rol
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(8), // Duración del token
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<AuthReponseDto> RegisterAsync(UserDto registerDto)
    {
        //validar si el Email existe
        var users = await _userRepository.GetAllUser();
        var existingUser = users.FirstOrDefault(u => u.Email == registerDto.Email);
        if (existingUser != null)
        {
            Console.WriteLine("El email ya está registrado");
        }
        
        //Parsear el rol de string a el enum
        if (!Enum.TryParse<UserRole>(registerDto.Role, true, out var userRole))
        {
            userRole = UserRole.User;//Rol por defecto si es invalido
        }
        
        //Hashear la contraseña
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.PasswordHash);

        var user = new User
        {
            Name = registerDto.Name,
            LasName = registerDto.LasName,
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            Role = userRole,
            PasswordHash = passwordHash
        };

        var newUser = await _userRepository.AddUser(user);
        
        //crear un dto de login
        var loginDto = new UserLoginDto { Email = registerDto.UserName, Pasword = registerDto.PasswordHash };
        return await LoginAsync(loginDto);
    }

    public async Task<AuthReponseDto> LoginAsync(UserLoginDto loginDto)
    {
        //validar el email y la contraseña
        var users = await _userRepository.GetAllUser();
        var user = users.FirstOrDefault(u => u.Email == loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Pasword, user.PasswordHash))
        {
            Console.WriteLine("Credenciales inválidas");
        }
        
        //Generar token JWT
        var token = GenerateJwtToken(user);
        var userDto = MapUserToDto(user);

        return new AuthReponseDto { Token = token, User = userDto };
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllUser();
        return users.Select(MapUserToDto).ToList();
    }


    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            Console.WriteLine("Usuario no encontrado");
        }
        return MapUserToDto(user);
    }

    public async Task UpdateAsync(int id, UserDto userDto)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            Console.WriteLine("Usuario no encontrado");
        }

        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
            
        if (Enum.TryParse<UserRole>(userDto.Role, true, out var userRole))
        {
            user.Role = userRole;
        }

        await _userRepository.UpdateUser(user);
    }

    public async Task DeleteAsync(int id)
    {
        var deletedUser = await _userRepository.DeleteUser(id);
        if (deletedUser == null)
        {
            Console.WriteLine("Usuario no encontrado para eliminar");
        }
    }
}
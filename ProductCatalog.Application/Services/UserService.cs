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
    
    // MÉTODO PARA MAPEAR USER A DTO
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
    
    // MÉTODO PARA GENERAR EL TOKEN JWT
    private string GenerateJwtToken(User user)
    {
        // Leer configuración JWT desde appsettings.json
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] 
                                                                  ?? throw new InvalidOperationException("Configuración JWT 'Key' no encontrada")));
            
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Configuración JWT 'Issuer' no encontrada");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Configuración JWT 'Audience' no encontrada");

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Crear claims (información que irá dentro del token)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Rol
        };

        // Crear token JWT
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(8), // Duración del token
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    // REGISTRO DE USUARIO
    public async Task<AuthReponseDto> RegisterAsync(UserDto registerDto)
    {
        // Validar si el Email ya existe
        var users = await _userRepository.GetAllUser();
        var existingUser = users.FirstOrDefault(u => u.Email == registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // Parsear el rol de string a enum (si falla, asignar User por defecto)
        if (!Enum.TryParse<UserRole>(registerDto.Role, true, out var userRole))
        {
            userRole = UserRole.User;
        }

        // Hashear la contraseña antes de guardar
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

        // Crear DTO de login para devolver el token inmediatamente después de registrar
        var loginDto = new UserLoginDto 
        { 
            Email = registerDto.Email, 
            Pasword = registerDto.PasswordHash 
        };

        return await LoginAsync(loginDto);
    }
    
    // LOGIN DE USUARIO
    public async Task<AuthReponseDto> LoginAsync(UserLoginDto loginDto)
    {
        // Validar email y contraseña
        var users = await _userRepository.GetAllUser();
        var user = users.FirstOrDefault(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Pasword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        // Generar token JWT
        var token = GenerateJwtToken(user);
        var userDto = MapUserToDto(user);

        return new AuthReponseDto { Token = token, User = userDto };
    }
    
    // OBTENER TODOS LOS USUARIOS
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllUser();
        return users.Select(MapUserToDto).ToList();
    }
    
    // OBTENER USUARIO POR ID
    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado");
        }
        return MapUserToDto(user);
    }
    
    // ACTUALIZAR USUARIO
    public async Task UpdateAsync(int id, UserDto userDto)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado");
        }

        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
            
        if (Enum.TryParse<UserRole>(userDto.Role, true, out var userRole))
        {
            user.Role = userRole;
        }

        await _userRepository.UpdateUser(user);
    }
    
    // ELIMINAR USUARIO
    public async Task DeleteAsync(int id)
    {
        var deletedUser = await _userRepository.DeleteUser(id);
        if (deletedUser == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado para eliminar");
        }
    }
}

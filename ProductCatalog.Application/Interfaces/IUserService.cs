using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Interfaces;

public interface IUserService
{
    Task<AuthReponseDto> RegisterAsync(UserDto registerDto);
    Task<AuthReponseDto> LoginAsync(UserLoginDto loginDto);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task UpdateAsync(int id, UserDto userDto);
    Task DeleteAsync(int id);
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    // REGISTRO DE USUARIO
    [HttpPost("register")]
    [AllowAnonymous] // Permite registro sin autenticación
    public async Task<IActionResult> Register([FromBody] UserDto registerDto)
    {
        try
        {
            var response = await _userService.RegisterAsync(registerDto);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex}, Error durante el registro");
            return StatusCode(500, new { message = "Ocurrió un error interno" });
        }
    }
    
    // LOGIN DE USUARIO
    [HttpPost("login")]
    [AllowAnonymous] // Permite login sin autenticación
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var response = await _userService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error durante el inicio de sesión: {ex}");
            return StatusCode(500, new { message = "Ocurrió un error interno" });
        }
    }
    
    // LISTAR TODOS LOS USUARIOS (SOLO ADMIN)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuarios: {ex}");
            return StatusCode(500, new { message = "Ocurrió un error interno" });
        }
    }
    
    // OBTENER USUARIO POR ID (SOLO ADMIN)
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuario: {ex}");
            return StatusCode(500, new { message = "Ocurrió un error interno" });
        }
    }

    
    // ACTUALIZAR USUARIO
    [HttpPut("{id}")]
    [Authorize] // Cualquier usuario autenticado puede actualizar su información
    public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
    {
        try
        {
            await _userService.UpdateAsync(id, userDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar usuario: {ex}");
            return StatusCode(500, new { message = "Ocurrió un error interno" });
        }
    }
    
    // ELIMINAR USUARIO (SOLO ADMIN)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar usuario: {ex}");
            return StatusCode(500, new { message = "Ocurrió un error interno" });
        }
    }
}

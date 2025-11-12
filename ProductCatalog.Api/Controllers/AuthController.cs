using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AuthController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    //Registro de usuario
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto registerDto)
    {
        if (registerDto == null)
            return BadRequest("Datos de registro inválidos.");

        var result = await _userService.RegisterAsync(registerDto);
        return Ok(result);
    }

    //Login de usuario
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        if (loginDto == null)
            return BadRequest("Datos de inicio de sesión inválidos.");

        var result = await _userService.LoginAsync(loginDto);
        return Ok(result);
    }

    //Refresh token
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        if (string.IsNullOrEmpty(request?.RefreshToken))
            return BadRequest("El token de actualización es requerido.");

        var result = await _authService.RefreshTokenAsync(request);
        return Ok(result);
    }

    //Logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { message = "Sesión cerrada correctamente." });
    }
}

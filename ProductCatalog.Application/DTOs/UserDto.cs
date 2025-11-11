using System.Text.Json.Serialization;

namespace ProductCatalog.Application.DTOs;

public class UserDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }

    [JsonPropertyName("passwordHash")]
    public string Password { get; set; }

    public string Role { get; set; }
}

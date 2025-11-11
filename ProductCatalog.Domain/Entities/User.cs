using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductCatalog.Domain.Enums;

namespace ProductCatalog.Domain.Entities;

public class User
{
    [Key] public int Id { get; set; }
    [Column(TypeName = "varchar(100)")] public string Name { get; set; }
    [Column(TypeName = ("varchar(100)"))] public string LastName { get; set; }
    [Column(TypeName = ("varchar(50)"))] public string Email { get; set; }
    [Column(TypeName = ("varchar(50)"))] public string UserName { get; set; }
    [Column(TypeName = ("varchar(200)"))] public string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
}
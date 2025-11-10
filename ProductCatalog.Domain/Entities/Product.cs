using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Domain.Entities;

public class Product
{
    [Key] public int Id { get; set; }
    [Column(TypeName = "varchar(150)")] public string Name { get; set; }
    [Column(TypeName = "varchar(500)")] public string Description { get; set; }
    public int Quantity { get; set; }
    [Column(TypeName = "decimal(5,2)")] public float Price { get; set; }
}
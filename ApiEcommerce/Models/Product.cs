using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//https://learn.microsoft.com/es-mx/ef/core/modeling/relationships
namespace ApiEcommerce.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]   // definir precision y escala en base de datos

    public decimal Price { get; set; }
    public string? ImgUrl { get; set; }
    public string? ImgUrlLocal { get; set; } 
    [Required]
    public string SKU { get; set; } = string.Empty; //  Prod-001-BLK-M
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; } = null;

    // Foreign Key
    
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    // Navigation Property
    public required Category Category { get; set; }


}

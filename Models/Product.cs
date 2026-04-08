using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTalles.Ecommerce.WebAPI.Models;

public class Product
{
    [Key] public int ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)] [MinLength(10)] public string Description { get; set; } = string.Empty;

    [Required] 
    [Range(0, 10000)] 
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Url] [MaxLength(1000)] public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Sku { get; set; } = string.Empty; // PROD-001-BLK-M

    [Required] [Range(0, 10000)] public int Stock { get; set; }

    [Required] public DateTime CreationDate { get; set; } = DateTime.Now;

    public DateTime? UpdateDate { get; set; } = null;

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")] public required Category Category { get; set; }
}
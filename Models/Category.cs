using System.ComponentModel.DataAnnotations;

namespace DevTalles.Ecommerce.WebAPI.Models
{
    /// <summary>
    /// Represents a category within the e-commerce system. A category is used to
    /// group and organize products, facilitating navigation and discovery by users.
    /// </summary>
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDate { get; set; }
    }
}

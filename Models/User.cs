using System.ComponentModel.DataAnnotations;

namespace DevTalles.Ecommerce.WebAPI.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Username { get; set; }
    
    public string? Password { get; set; }

     public string? Role { get; set; }
     
     public bool IsActive { get; set; } = true;
}
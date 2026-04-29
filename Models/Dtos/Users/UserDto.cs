namespace DevTalles.Ecommerce.WebAPI.Models.Dtos.Users;

public class UserDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Username { get; set; }
    
    public string? Password { get; set; }

    public string? Role { get; set; }
     
    public bool IsActive { get; set; } = true;
}
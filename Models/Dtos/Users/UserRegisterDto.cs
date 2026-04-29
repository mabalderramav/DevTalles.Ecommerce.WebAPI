namespace DevTalles.Ecommerce.WebAPI.Models.Dtos.Users;

public class UserRegisterDto
{
    public string? Id { get; set; }

    public required string Username { get; set; }
    
    public required string Password { get; set; }
    
    public string? Name { get; set; }

    public string? Role { get; set; }
     
    public bool IsActive { get; set; } = true;
}
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Models.Dtos.Users;

namespace DevTalles.Ecommerce.WebAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User? GetUser(int userId);
        bool IsUniqueUser(string username);
        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
        Task<User?> Register(CreateUserDto createUserDto);
    }
}

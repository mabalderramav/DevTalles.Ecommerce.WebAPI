using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevTalles.Ecommerce.WebAPI.Data;
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Models.Dtos.Users;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DevTalles.Ecommerce.WebAPI.Repository
{
    public class UserRepository(ApplicationDbContext db, IConfiguration configuration) : IUserRepository
    {
        private readonly string? _secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        public ICollection<User> GetUsers()
        {
            return db.Users.OrderBy(u => u.Username).ToList();
        }

        public User? GetUser(int userId)
        {
            return db.Users.FirstOrDefault(u => u.Id == userId);
        }

        public bool IsUniqueUser(string username)
        {
            return !db.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
        {
            if (string.IsNullOrEmpty(userLoginDto.Username))
            {
                return new UserLoginResponseDto
                {
                    Token = string.Empty,
                    User = null,
                    Message = "Username is required"
                };
            }
            if (string.IsNullOrEmpty(userLoginDto.Password))
            {
                return new UserLoginResponseDto
                {
                    Token = string.Empty,
                    User = null,
                    Message = "Password is required"
                };
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());
            if (user == null)
            {
                return new UserLoginResponseDto
                {
                    Token = string.Empty, User = null, Message = "Username not found"
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
            {
                return new UserLoginResponseDto()
                {
                    Token = string.Empty,
                    User = null,
                    Message = "Invalid credentials"
                };
            }

            var handlerToken = new JwtSecurityTokenHandler();
            if (string.IsNullOrWhiteSpace(_secretKey))
            {
                throw new InvalidOperationException("Secret key is not configured.");
            }

            var key = Encoding.UTF8.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username),
                    new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
                ]),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handlerToken.CreateToken(tokenDescriptor);
            var tokenString = handlerToken.WriteToken(token);

            return new UserLoginResponseDto()
            {
                Token = tokenString,
                User = new UserRegisterDto()
                {
                    Username = user.Username,
                    Name = user.Name,
                    Role = user.Role,
                    Password = user.Password ?? string.Empty
                },
                Message = "Login successfully"
            };
        }

        public async Task<User?> Register(CreateUserDto createUserDto)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            var user = new User
            {
                Username = createUserDto.Username ?? "Not Username",
                Password = encryptedPassword,
                Name = createUserDto.Name,
                Role = createUserDto.Role
            };
            db.Users.Add(user);
            var result = await db.SaveChangesAsync();
            return result <= 0 ? null : user;
        }
    }
}

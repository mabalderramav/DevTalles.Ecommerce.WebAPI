using AutoMapper;
using DevTalles.Ecommerce.WebAPI.Models.Dtos.Users;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace DevTalles.Ecommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(
        IUserRepository userRepository,
        IMapper mapper) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetUsers()
        {
            var users = userRepository.GetUsers();
            var usersDto = mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }

        [HttpGet("{userId:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(int userId)
        {
            var user = userRepository.GetUser(userId);
            if (user == null) return NotFound($"User with ID {userId} not found.");
            var userDto = mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto? createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid) return BadRequest("User data is null or invalid.");
            if (string.IsNullOrWhiteSpace(createUserDto.Username) || string.IsNullOrWhiteSpace(createUserDto.Password))
            {
                return BadRequest("Username or password is missing.");
            }

            if (!userRepository.IsUniqueUser(createUserDto.Username))
            {
                return BadRequest("Username is already in use.");
            }

            var newUser = await userRepository.Register(createUserDto);
            if (newUser == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Something went wrong while saving the user.");
            }

            var userDto = mapper.Map<UserDto>(newUser);
            return CreatedAtRoute("GetUser", new { userId = userDto.Id }, userDto);
        }

        [HttpPost("Login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto? userLoginDto)
        {
            if (userLoginDto == null || !ModelState.IsValid) return BadRequest("Login data is null or invalid.");
            var loginResponse = await userRepository.Login(userLoginDto);
            if (string.IsNullOrEmpty(loginResponse.Token))
            {
                return Unauthorized(loginResponse.Message);
            }

            return Ok(loginResponse);
        }
    }
}

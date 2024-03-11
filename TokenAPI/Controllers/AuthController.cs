using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TokenAPI.DTO;
using TokenAPI.Models;
using TokenAPI.Records;
using TokenAPI.Services.JWT;
using TokenAPI.Services.User;

namespace TokenAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        ApplicationContext _context;
        private readonly IUserService _userService;
        public AuthController(ApplicationContext context, IUserService userService) 
        {
            _context = context; 
            _userService = userService;
        }        

        [HttpPost("Login")]
        public IResult Login([FromBody] UserInfo user,
        [FromServices] IUserService userService, [FromServices] IJwtService jwtService)
        {
            if(String.IsNullOrEmpty(user.Username))
            {
                return  Results.Ok(new { error = "Username is empty" });
            }
            if(String.IsNullOrEmpty(user.Password))
            {
                return Results.Ok(new { error = "Password is empty" });
            }
            UserDto storedUser = userService.GetUser(user.Username);            
            if(storedUser == null)
            {
                return Results.Ok(new {error = "User not found"});
            }
            if (!userService.IsAuthenticated(user.Password, storedUser.PasswordHash))
            {                
                return Results.Unauthorized();
            }
            var tokenString = jwtService.GenerateToken(storedUser);
            return Results.Ok(new { token = tokenString });
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register([FromBody] UserRegister user,
        [FromServices] IUserService userService, [FromServices] IJwtService jwtService)
        {
            if(String.IsNullOrEmpty(user.Username))
            { return Ok(new { error = "Username is empty" }); }
            if (String.IsNullOrEmpty(user.Email))
            { return Ok(new { error = "Email is empty" }); }
            if (String.IsNullOrEmpty(user.Password))
            { return Ok(new { error = "Password is empty" }); }
            if (String.IsNullOrEmpty(user.PasswordConfirmation))
            { return Ok(new { error = "PasswodConfirmation is empty" }); }
            
            if(user.Password.Trim() != user.PasswordConfirmation.Trim())
            {
                return Ok(new { error = "Passwords do not match" });
            }
            string passwordHash = CreatePasswordHash(user.Password);
            UserDto storedUser = userService.GetUser(user.Username);
            if (storedUser != null) 
            {
                return Ok(new { error = "User already exists" });
            }            
            UserDto userDto = new UserDto(Guid.NewGuid(), user.Username, user.Email, passwordHash, "User");

            var userId = userService.CreateUser(userDto);
            if (userId != null) 
            {
                var tokenString = jwtService.GenerateToken(userDto);
                return Ok(new { token = tokenString });
            }
            else
            {
                return BadRequest();
            }
            return Ok(user);
        }
        private string CreatePasswordHash(string password)
        {                        
                 return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

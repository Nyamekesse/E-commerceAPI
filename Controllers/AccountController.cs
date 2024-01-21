using E_commerceAPI.Data;
using E_commerceAPI.Models;
using E_commerceAPI.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_commerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IConfiguration configuration, ApplicationDBContext context) : ControllerBase
    {

        string secretKey = Environment.GetEnvironmentVariable("SECRET")!;
        private string CreateJWToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("JWTConfig:Issuer")!,
                audience: configuration.GetValue<string>("JWTConfig:Audience")!,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpPost("register")]
        public IActionResult Register(UserCreateDTO userCreateDTO)
        {

            var emailCount = context.Users.Count(u => u.Email == userCreateDTO.Email);
            if (emailCount > 0)
            {
                ModelState.AddModelError("Email", "The email address already exists");
                return BadRequest(ModelState);
            }

            var passwordHasher = new PasswordHasher<User>();
            var encryptedPassword = passwordHasher.HashPassword(new User(), userCreateDTO.Password);

            User user = new()
            {
                FirstName = userCreateDTO.FirstName,
                LastName = userCreateDTO.LastName,
                Email = userCreateDTO.Email,
                Phone = userCreateDTO.Phone ?? "",
                Address = userCreateDTO.Address,
                Password = encryptedPassword,
                Role = "client",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.SaveChanges();

            var jwt = CreateJWToken(user);

            UserProfileDTO profile = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            var response = new
            {
                Token = jwt,
                User = profile
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO userLoginDTO)
        {

            var user = context.Users.FirstOrDefault(u => u.Email == userLoginDTO.Email);
            if (user is null)
            {
                ModelState.AddModelError("Email", "The email address already exists");
                return BadRequest(ModelState);
            }
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(new User(), user.Password, userLoginDTO.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "Incorrect password");
                return BadRequest(ModelState);
            }

            var jwt = CreateJWToken(user);
            UserProfileDTO profile = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            var response = new
            {
                Token = jwt,
                User = profile
            };

            return Ok(response);
        }
    }
}

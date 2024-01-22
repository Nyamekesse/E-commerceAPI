using E_commerceAPI.Data;
using E_commerceAPI.Models;
using E_commerceAPI.Models.DTO;
using E_commerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
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
    public class AccountController(IConfiguration configuration, ApplicationDBContext context, EmailSender emailSender) : ControllerBase
    {

        readonly string secretKey = Environment.GetEnvironmentVariable("SECRET")!;
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


        [Authorize]
        [HttpGet("authorize-admin")]
        public IActionResult AuthorizeAuthenticatedUsers()
        {
            return Ok("You are Authorized.");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("authorize-authenticated-users")]
        public IActionResult AuthorizeAdmin()
        {
            return Ok("You are Authorized.");
        }

        [Authorize(Roles = "admin, seller")]
        [HttpGet("authorize-admin-seller")]
        public IActionResult AuthorizeAdminSeller()
        {
            return Ok("You are Authorized.");
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] string email)


        {

            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            var oldPasswordReset = context.PasswordResets.FirstOrDefault(r => r.Email == email);
            if (oldPasswordReset is not null)
            {
                context.Remove(oldPasswordReset);
            }

            string token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
            var passwordReset = new PasswordReset()
            {
                Email = email,
                Token = token,
                CreatedAt = DateTime.UtcNow
            };
            context.PasswordResets.Add(passwordReset);
            context.SaveChanges();

            string emailSubject = "Password Reset";
            string messageBody = $"We received your password reset request please copy the following token and paste it in the password reset box\n\nToken: {token}";
            string recipient = email;

            emailSender.SendEmail(subject: emailSubject, body: messageBody, receiver: recipient);

            return Ok();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(string token, string password)
        {
            var passwordReset = context.PasswordResets.FirstOrDefault(r => r.Token == token);
            if (passwordReset is null)
            {
                ModelState.AddModelError("Token", "Wrong or expired token");
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }

}

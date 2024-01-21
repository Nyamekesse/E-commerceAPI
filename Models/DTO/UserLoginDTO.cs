using System.ComponentModel.DataAnnotations;

namespace E_commerceAPI.Models.DTO
{
    public class UserLoginDTO
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}

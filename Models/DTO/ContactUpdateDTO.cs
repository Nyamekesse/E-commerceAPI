using System.ComponentModel.DataAnnotations;

namespace E_commerceAPI.Models.DTO
{
    public class ContactUpdateDTO
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = "";
        [Required, MaxLength(50)]
        public string LasttName { get; set; } = "";
        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; } = "";

        [MaxLength(20)]
        public string? Phone { get; set; }
        public int SubjectId { get; set; }
        [Required, MinLength(20), MaxLength(4000)]
        public string Message { get; set; } = "";
    }
}

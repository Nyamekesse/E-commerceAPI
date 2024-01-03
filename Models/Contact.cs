using System.ComponentModel.DataAnnotations;

namespace E_commerceAPI.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; } = "";
        [MaxLength(50)]
        public string LasttName { get; set; } = "";
        [MaxLength(50)]
        public string Email { get; set; } = "";

        [MaxLength(20)]
        public string Phone { get; set; } = "";
        [MaxLength(50)]
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}

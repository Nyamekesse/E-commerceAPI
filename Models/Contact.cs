using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerceAPI.Models
{
    public class Contact
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; } = "";
        [MaxLength(50)]
        public string LasttName { get; set; } = "";
        [MaxLength(50)]
        public string Email { get; set; } = "";

        [MaxLength(20)]
        public string Phone { get; set; } = "";
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public required Subject Subject { get; set; }
        public string Message { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}

using System.ComponentModel.DataAnnotations;

namespace E_commerceAPI.Models.DTO
{
    public class ProductCreateDTO
    {

        [Required, MaxLength(50)]
        public string Name { get; set; } = "";
        [Required, MaxLength(50)]
        public string Brand { get; set; } = "";
        [Required, MaxLength(50)]
        public string Category { get; set; } = "";
        [Required]
        public decimal Price { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        public IFormFile? ImageFile { get; set; }
    }
}

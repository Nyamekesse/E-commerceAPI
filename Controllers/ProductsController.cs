using AutoMapper;
using E_commerceAPI.Data;
using E_commerceAPI.Models;
using E_commerceAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace E_commerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ApplicationDBContext context, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetProductById(int id)
        {

            var product = _context.Products.FirstOrDefault(product => product.Id == id);
            if (product == null) { return NotFound(); }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductCreateDTO productDTO)
        {
            Product newProduct = _mapper.Map<Product>(productDTO);
            if (productDTO.ImageFile != null)
            {

                string fileName = newProduct.Id.ToString() + Path.GetExtension(productDTO.ImageFile.FileName);
                string filePath = @"wwwroot\images\products\" + fileName;
                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                FileInfo file = new(directoryLocation);
                if (file.Exists)
                {
                    file.Delete();
                }
                using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                    productDTO.ImageFile.CopyTo(fileStream);
                }
                newProduct.ImageFileName = fileName;
            }
            else
            {
                newProduct.ImageFileName = "https://placehold.co/600x400/EEE/31343C";
            }

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            return Ok(newProduct);
        }
    }
}

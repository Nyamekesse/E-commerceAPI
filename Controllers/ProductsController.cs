using AutoMapper;
using E_commerceAPI.Data;
using E_commerceAPI.Models;
using E_commerceAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult CreateProduct([FromForm] ProductCreateDTO productDTO)
        {
            Product newProduct = _mapper.Map<Product>(productDTO);
            _context.Products.Add(newProduct);
            _context.SaveChanges();
            if (productDTO.ImageFile != null)
            {
                string fileName = newProduct.Id + Path.GetExtension(productDTO.ImageFile.FileName);
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

            _context.Products.Update(newProduct);
            _context.SaveChanges();

            return Ok(newProduct);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateProduct(int id, [FromForm] ProductUpdateDTO updateDTO)
        {
            if (updateDTO == null) return BadRequest();
            var product = _context.Products.AsNoTracking().FirstOrDefault(product => product.Id == id);
            if (product == null) return NotFound();
            Product productToUpdate = _mapper.Map<Product>(updateDTO);
            if (updateDTO.ImageFile != null)
            {
                string fileName = productToUpdate.Id + Path.GetExtension(updateDTO.ImageFile.FileName);
                string filePath = @"wwwroot\images\products\" + fileName;
                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                FileInfo file = new(directoryLocation);
                if (file.Exists)
                {
                    file.Delete();
                }
                using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                    updateDTO.ImageFile.CopyTo(fileStream);
                }
                productToUpdate.ImageFileName = fileName;
            }
            _context.Products.Update(productToUpdate);
            _context.SaveChanges();
            return Ok(productToUpdate);
        }
    }
}

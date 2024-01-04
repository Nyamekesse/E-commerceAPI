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
    public class ContactController(ApplicationDBContext context, IMapper mapper) : ControllerBase
    {
        private readonly ApplicationDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult GetContacts()
        {
            var contacts = _context.Contacts.ToList();
            return Ok(contacts);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetContactById(int id)
        {
            var contact = context.Contacts.Find(id);
            if (contact == null) return NotFound();
            return Ok(contact);
        }

        [HttpPost]
        public IActionResult CreateContact(ContactRequestDTO contactRequestDTO)
        {
            Contact contact = _mapper.Map<Contact>(contactRequestDTO);
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return Ok(contact);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateContact(int id, ContactUpdateDTO contactUpdateDTO)
        {
            var contact = _context.Contacts.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (contact == null) return NotFound();
            Contact contactToUpdate = _mapper.Map<Contact>(contactUpdateDTO);
            _context.Contacts.Update(contactToUpdate);
            _context.SaveChanges();
            return Ok(contactToUpdate);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteContactById(int id)
        {
            //var contact = _context.Contacts.Find(id);
            //if (contact == null) return NotFound();
            //_context.Contacts.Remove(contact);
            //_context.SaveChanges();
            try
            {
                var contact = new Contact() { Id = id };
                _context.Contacts.Remove(contact);
                _context.SaveChanges();
            }
            catch
            {
                return NotFound();
            }
            return Ok();

        }
    }
}

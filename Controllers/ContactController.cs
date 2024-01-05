using AutoMapper;
using E_commerceAPI.Data;
using E_commerceAPI.Models;
using E_commerceAPI.Models.DTO;
using E_commerceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController(ApplicationDBContext context, IMapper mapper, EmailSender emailSender) : ControllerBase
    {
        private readonly ApplicationDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet("subjects")]
        public IActionResult GetSubjects()
        {
            var listSubjects = _context.Subjects.ToList();
            return Ok(listSubjects);
        }

        [HttpGet]
        public IActionResult GetContacts()
        {
            var contacts = _context.Contacts.Include(contacts => contacts.Subject).ToList();
            return Ok(contacts);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetContactById(int id)
        {
            var contact = _context.Contacts.Include(contact => contact.Subject).FirstOrDefault(contact => contact.Id == id);
            if (contact is null) return NotFound();
            return Ok(contact);
        }

        [HttpPost]
        public IActionResult CreateContact(ContactCreateDTO contactRequestDTO)
        {
            var subject = _context.Subjects.AsNoTracking().FirstOrDefault(subject => subject.Id == contactRequestDTO.SubjectId);

            if (subject is null)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }
            Contact contact = _mapper.Map<Contact>(contactRequestDTO);

            _context.Contacts.Add(contact);
            _context.SaveChanges();

            string emailSubject = "Account Created";
            string messageBody = "This is the details of the account created";
            string recipient = contact.Email;

            emailSender.SendEmail(subject: emailSubject, body: messageBody, receiver: recipient);

            return Ok(contact);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateContact(int id, ContactUpdateDTO contactUpdateDTO)
        {
            var subject = _context.Subjects.AsNoTracking().First(u => u.Id == contactUpdateDTO.SubjectId);
            if (subject is null)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }
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
                var contact = new Contact() { Id = id, Subject = new Subject() };
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

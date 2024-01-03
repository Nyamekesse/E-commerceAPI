using E_commerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerceAPI.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}

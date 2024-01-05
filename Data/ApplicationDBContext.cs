using E_commerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerceAPI.Data
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Subject>().HasData(
                new Subject
                {
                    Id = 1,
                    Name = "Order Status",
                },
                new Subject
                {
                    Id = 2,
                    Name = "Refund Request",
                },
                new Subject
                {
                    Id = 3,
                    Name = "Job Application",
                },
                new Subject
                {
                    Id = 4,
                    Name = "Other"
                }

                );
        }
    }
}

using CodeHelper.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeHelper.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options, IWebHostEnvironment webHostEnvironment) : base(options)
        {
            if (webHostEnvironment.IsProduction())
            {
                Console.WriteLine("Init Database");
                Database.EnsureCreated();
                Console.WriteLine("End init Database");
            }
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
    }
}

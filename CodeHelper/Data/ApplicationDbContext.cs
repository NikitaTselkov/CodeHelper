using CodeHelper.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace CodeHelper.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            var RDC = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (RDC != null && !RDC.Exists())
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

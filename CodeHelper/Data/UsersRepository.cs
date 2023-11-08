using CodeHelper.Data.Repository;
using CodeHelper.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodeHelper.Data
{
    public class UsersRepository : Repository<User>
    {
        private readonly ApplicationDbContext _dbContext;
        public UsersRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Users.Include(i => i.Answers).Include(i => i.Questions);
        }
    }
}

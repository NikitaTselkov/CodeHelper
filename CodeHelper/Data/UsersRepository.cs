using CodeHelper.Data.Repository;
using CodeHelper.Models.Domain;

namespace CodeHelper.Data
{
    public class UsersRepository : Repository<User>
    {
        private readonly ApplicationDbContext _dbContext;
        public UsersRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

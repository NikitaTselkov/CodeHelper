using CodeHelper.Data.Repository;
using CodeHelper.Models.Domain;

namespace CodeHelper.Data
{
    public class LikesRepository : Repository<Like>
    {
        private readonly ApplicationDbContext _dbContext;
        public LikesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

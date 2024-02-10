using CodeHelper.Data.Repository;
using CodeHelper.Models.Domain;

namespace CodeHelper.Data
{
    public class QuestionsRepository : Repository<Question>
    {
        private readonly ApplicationDbContext _dbContext;
        public QuestionsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }  
    }
}

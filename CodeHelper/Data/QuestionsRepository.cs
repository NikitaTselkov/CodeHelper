using CodeHelper.Data.Repository;
using CodeHelper.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodeHelper.Data
{
    public class QuestionsRepository : Repository<Question>
    {
        private readonly ApplicationDbContext _dbContext;
        public QuestionsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Questions.Include(i => i.Author);
        }
    }
}

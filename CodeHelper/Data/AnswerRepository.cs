using CodeHelper.Data.Repository;
using CodeHelper.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodeHelper.Data
{
    public class AnswerRepository : Repository<Answer>
    {
        private readonly ApplicationDbContext _dbContext;
        public AnswerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Answers.Include(i => i.User);
        }
    }
}

﻿using CodeHelper.Data.Repository;
using CodeHelper.Models.Domain;

namespace CodeHelper.Data
{
    public class TagRepository : Repository<Tag>
    {
        private readonly ApplicationDbContext _dbContext;
        public TagRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

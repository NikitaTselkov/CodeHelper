﻿using Korzh.EasyQuery.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CodeHelper.Data.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = _dbContext.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IQueryable<T> SearchByText(string text, Expression<Func<T, bool>>? filter = null, int pageOffset = 0, int rowsCount = 0, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrEmpty(text))
                query = query.FullTextSearchQuery(text);

            foreach (var includeExpression in includeProperties)
            {
                query = query.Include(includeExpression);
            }

            if (filter != null)
                query = query.Where(filter);

            if (rowsCount > 0)
                return query.Skip(pageOffset).Take(rowsCount);
            else
                return query.Skip(pageOffset);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>>? filter, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            foreach (var includeExpression in includeProperties)
            {
                query = query.Include(includeExpression);
            }

            if (filter != null)
                query = query.Where(filter);

            return query;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>>? filter, int pageOffset = 0, int rowsCount = 0, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            foreach (var includeExpression in includeProperties)
            {
                query = query.Include(includeExpression);
            }

            if (filter != null)
                query = query.Where(filter);

            if (rowsCount > 0)
                return query.Skip(pageOffset).Take(rowsCount);
            else
                return query.Skip(pageOffset);
        }

        public IEnumerable<T> GetAll(int pageOffset = 0, int rowsCount = 0, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            foreach (var includeExpression in includeProperties)
            {
                query = query.Include(includeExpression);
            }

            if (rowsCount > 0)
                return query.Skip(pageOffset).Take(rowsCount);
            else
                return query.Skip(pageOffset);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}

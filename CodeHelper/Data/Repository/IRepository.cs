﻿using System.Linq.Expressions;

namespace CodeHelper.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> GetAll(int pageOffset = 0, int rowsCount = 0, params Expression<Func<T, object>>[] includeProperties);
        public IQueryable<T> Get(Expression<Func<T, bool>>? filter, params Expression<Func<T, object>>[] includeProperties);
        public IQueryable<T> Get(Expression<Func<T, bool>>? filter, int pageOffset = 0, int rowsCount = 0, params Expression<Func<T, object>>[] includeProperties);
        public IQueryable<T> SearchByText(string text, Expression<Func<T, bool>>? filter = null, int pageOffset = 0, int rowsCount = 0, params Expression<Func<T, object>>[] includeProperties);
        void Add(T entity);
        void Remove(T entity);
        void Save();
        void Update(T entity);

    }
}

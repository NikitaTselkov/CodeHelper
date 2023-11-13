using System.Linq.Expressions;

namespace CodeHelper.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
        public IQueryable<T> Get(Expression<Func<T, bool>>? filter, params Expression<Func<T, object>>[] includeProperties);
        void Add(T entity);
        void Remove(T entity);
        void Save();
        void Update(T entity);

    }
}

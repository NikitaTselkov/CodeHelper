using System.Linq.Expressions;

namespace CodeHelper.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll(int pageOffset = 0, int rowsCount = 0, params Expression<Func<T, object>>[] includeProperties);
        public IQueryable<T> Get(Expression<Func<T, bool>>? filter, params Expression<Func<T, object>>[] includeProperties);
        public int GetRowsCount();
        void Add(T entity);
        void Remove(T entity);
        void Save();
        void Update(T entity);

    }
}

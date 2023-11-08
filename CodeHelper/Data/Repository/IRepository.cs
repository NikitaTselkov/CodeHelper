using System.Linq.Expressions;

namespace CodeHelper.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties);
        void Add(T entity);
        void Remove(T entity);
        void Save();
        void Update(T entity);

    }
}

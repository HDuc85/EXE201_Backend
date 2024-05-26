using System.Linq.Expressions;

namespace Service.Interface
{
    public interface IRepository<T> where T : class
    {
        Task Commit();
        void Delete(Expression<Func<T, bool>> expression);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression = null);
        Task<T> GetById(object id);
        Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression = null);
        Task Insert(IEnumerable<T> entities);
        Task Insert(T entity);
        void Update(T entity);
    }
}

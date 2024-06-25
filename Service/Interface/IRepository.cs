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
        Task<IEnumerable<T>> GetPageSize(Expression<Func<T, bool>> expression = null, int pageIndex = 1, int pageSize = 5);
        Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression = null);
        Task Insert(IEnumerable<T> entities);
        Task Insert(T entity);
        void Update(T entity);
        void RemoveRange(IEnumerable<T> entities);
<<<<<<< Updated upstream
=======
        Task<IEnumerable<T>> GetListByCondition(Expression<Func<T, bool>> expression = null);
        IQueryable<T> GetAllWithCondition(Expression<Func<T, bool>> expression = null);
>>>>>>> Stashed changes
    }
}

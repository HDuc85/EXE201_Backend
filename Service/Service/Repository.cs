using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Service.Interface;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace Service.Service
{
    public class Repository<T> : IRepository<T> where T : class
    {
        PostgresContext _postgresContext;

        public Repository(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

       

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression = null)
        {
            if(expression == null) 
                return await _postgresContext.Set<T>().ToListAsync();
             return await _postgresContext.Set<T>().Where(expression).ToListAsync();
        }
<<<<<<< Updated upstream
        public async Task<IEnumerable<T>> GetPageSize(Expression<Func<T, bool>> expression = null,int pageIndex = 1, int pageSize = 5)
=======
        public IQueryable<T> GetAllWithCondition(Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
                return _postgresContext.Set<T>();
            return _postgresContext.Set<T>().Where(expression);
        }

        public async Task<IEnumerable<T>> GetPageSize(Expression<Func<T, bool>> expression = null, int pageIndex = 1, int pageSize = 5)
>>>>>>> Stashed changes
        {
            if (pageIndex == 0) pageIndex = 1;
            if (pageSize == 0) pageSize = 5;

            if (expression == null)
            {
                return await _postgresContext.Set<T>().Skip((pageIndex -1 ) * pageSize).Take(pageSize).ToListAsync();
            }
            return await _postgresContext.Set<T>().Where(expression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await _postgresContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression = null)
        {
            if(expression == null)
                return await _postgresContext.Set<T>().FirstOrDefaultAsync();
            return await _postgresContext.Set<T>().Where(expression).FirstOrDefaultAsync();
        }

        public void Delete(T entity)
        {
            EntityEntry entityEntry = _postgresContext.Entry<T>(entity);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public void Delete(Expression<Func<T, bool>> expression)
        {
           var entities = _postgresContext.Set<T>().Where(expression).ToList();
           if( entities.Count > 0 ) _postgresContext.Set<T>().RemoveRange(entities);
           

        }

        public async Task Insert(T entity)
        {
           await _postgresContext.Set<T>().AddAsync(entity);
            
        }

        public async Task Insert(IEnumerable<T> entities)
        {
           await _postgresContext.Set<T>().AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            EntityEntry entityEntry = _postgresContext.Entry<T>(entity);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }


        public virtual IQueryable<T> Table => _postgresContext.Set<T>();   

        public async Task Commit()
        {
           await _postgresContext.SaveChangesAsync();
        }
        public void RemoveRange(IEnumerable<T> entities) // Implement this method
        {
            _postgresContext.RemoveRange(entities);
        }
<<<<<<< Updated upstream
=======
        public async Task<IEnumerable<T>> GetListByCondition(Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
                return await _postgresContext.Set<T>().ToListAsync();
            return await _postgresContext.Set<T>().Where(expression).ToListAsync();
        }

>>>>>>> Stashed changes
    }
}

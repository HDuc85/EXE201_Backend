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
    }
}

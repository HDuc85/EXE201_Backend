using Data.Models;
using Service.Interface;

namespace Service.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        PostgresContext _postgresContext;

        public UnitOfWork(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

        public async Task CommitAsync()
        {
            await _postgresContext.SaveChangesAsync();
        }

    }
}

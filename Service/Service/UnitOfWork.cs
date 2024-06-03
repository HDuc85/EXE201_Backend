using Data.Models;
using Service.Interface;
using System.Diagnostics;

namespace Service.Service
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        PostgresContext _postgresContext;

        Repository<User> _repositoryUser;
        Repository<UserStatusLog> _repositoryUserStatusLog;
        Repository<Status> _repositoryStatus;
        Repository<Cart> _repositoryCart;
 

        private bool disposedValue;

        public UnitOfWork(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

        public Repository<User> RepositoryUser {  get { return _repositoryUser ??= new Repository<User>(_postgresContext); } }
        public Repository<UserStatusLog> RepositoryUserStatusLog { get { return _repositoryUserStatusLog ??= new Repository<UserStatusLog>(_postgresContext); } }
        public Repository<Status> RepositoryStatus { get { return _repositoryStatus ??= new Repository<Status>(_postgresContext); } }


        public async Task CommitAsync()
        {
            await _postgresContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _postgresContext.Dispose();
                }

               
                disposedValue = true;
            }
        }

       

        public void Dispose()
        {
         
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

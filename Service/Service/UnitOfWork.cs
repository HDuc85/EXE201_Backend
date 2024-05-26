using Data.Models;
using Service.Interface;
using System.Diagnostics;

namespace Service.Service
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        PostgresContext _postgresContext;

        Repository<User> _repositoryUser;
        Repository<UserToken> _repositoryUserToken;
        private bool disposedValue;

        public UnitOfWork(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

        public Repository<User> RepositoryUser {  get { return _repositoryUser ??= new Repository<User>(_postgresContext); } }
        public Repository<UserToken> RepositoryUserToken { get { return _repositoryUserToken ??= new Repository<UserToken>(_postgresContext); } }


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

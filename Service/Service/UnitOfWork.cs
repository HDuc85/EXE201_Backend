using Data.Models;
using Service.Interface;
using System.Diagnostics;

namespace Service.Service
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        PostgresContext _postgresContext;

        Repository<User> _repositoryUser;
        Repository<Product> _repositoryProduct;
        Repository<ProductVariant> _repositoryProductVariant;
        Repository<Size> _repositorySize;
        Repository<Brand> _repositoryBrand;
        Repository<Color> _repositoryColor;
        Repository<UserStatusLog> _repositoryUserStatusLog;
        Repository<Status> _repositoryStatus;
        private bool disposedValue;

        public UnitOfWork(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

        public Repository<User> RepositoryUser {  get { return _repositoryUser ??= new Repository<User>(_postgresContext); } }
        public Repository<UserStatusLog> RepositoryUserStatusLog { get { return _repositoryUserStatusLog ??= new Repository<UserStatusLog>(_postgresContext); } }
        public Repository<Status> RepositoryStatus { get { return _repositoryStatus ??= new Repository<Status>(_postgresContext); } }

        public Repository<Product> RepositoryProduct { get { return _repositoryProduct ??= new Repository<Product>(_postgresContext); } }

        public Repository<ProductVariant> RepositoryVariant { get { return _repositoryProductVariant ??= new Repository<ProductVariant>(_postgresContext); } }

        public Repository<Size> RepositorySize { get { return _repositorySize ??= new Repository<Size>(_postgresContext); } }

        public Repository<Brand> RepositoryBrand { get { return _repositoryBrand ??= new Repository<Brand>(_postgresContext); } }

        public Repository<Color> RepositoryColor { get { return _repositoryColor ??= new Repository<Color>(_postgresContext);
    }
}

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

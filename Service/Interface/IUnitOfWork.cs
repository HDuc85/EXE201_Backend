using Data.Models;
using Service.Service;

namespace Service.Interface
{
    public interface IUnitOfWork
    {
        Repository<User> RepositoryUser { get; }
        Repository<UserToken> RepositoryUserToken { get; }

        Task CommitAsync();
    }
}
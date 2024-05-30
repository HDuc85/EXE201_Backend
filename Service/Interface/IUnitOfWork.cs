using Data.Models;
using Service.Service;

namespace Service.Interface
{
    public interface IUnitOfWork
    {
        Repository<User> RepositoryUser { get; }
        Repository<UserStatusLog> RepositoryUserStatusLog { get; }
        Repository<Status> RepositoryStatus { get; }

        Task CommitAsync();
    }
}
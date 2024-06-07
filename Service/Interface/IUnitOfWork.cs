using Data.Models;
using Service.Service;

namespace Service.Interface
{
    public interface IUnitOfWork
    {
        Repository<User> RepositoryUser { get; }
        Repository<UserStatusLog> RepositoryUserStatusLog { get; }
        Repository<Status> RepositoryStatus { get; }
        Repository<Product> RepositoryProduct { get; }
        Repository<ProductVariant> RepositoryVariant { get; }
        Repository<Size> RepositorySize{ get; }
        Repository<Brand> RepositoryBrand { get; }
        Repository<Color> RepositoryColor { get; }
        Task CommitAsync();
    }
}
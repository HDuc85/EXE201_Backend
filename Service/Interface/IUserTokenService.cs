using Data.Models;

namespace Service.Interface
{
    public interface IUserTokenService
    {
        Task<UserToken> CheckRefreshToken(string code);
        Task SaveToken(UserToken userToken);
    }
}

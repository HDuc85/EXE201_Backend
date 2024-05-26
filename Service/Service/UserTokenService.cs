using Data.Models;
using Service.Interface;

namespace Service.Service
{
    public class UserTokenService : IUserTokenService
    {
        IUnitOfWork _unitOfWork;
        public UserTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SaveToken(UserToken userToken)
        {
           await _unitOfWork.RepositoryUserToken.Insert(userToken);
            await _unitOfWork.RepositoryUserToken.Commit();
        }
        
        public async Task<UserToken> CheckRefreshToken(string code)
        {
            return await _unitOfWork.RepositoryUserToken.GetSingleByCondition(u => u.CodeRefreshToken == code);
        }
    }
}

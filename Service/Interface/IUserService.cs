using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Authen;
using Data.ViewModel.User;

namespace Service.Interface
{
    public interface IUserService 
    {
        Task<ApiResult<User>> CheckLogin(LoginRequest loginRequest);
        Task<ApiResult<bool>> ComfirmEmail(string userId, string tokenConfirm);
        Task<User> FindById(Guid userId);
        Task<User> FindByUsername(string username);
        Task<ApiResult<bool>> ForgetPassword(string email, string host);
        Task<(ApiResult<User>, string)> Register(RegisterRequest registerRequest);
        Task<ApiResult<bool>> ResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task<bool> SendEmailComfirm(string Url, User user);
        Task<ApiResult<bool>> UpdateRole(UpdateRoleRequest updateRoleRequest);
        Task<ApiResult<User>> UpdateUser(UpdateUserRequest updateUserRequest);
    }
}
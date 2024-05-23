
using Service.ViewModel.System;
namespace Service.Repo
{
    public interface IAuthenService
    {
        public Task<string> Authencate(LoginRequest requset);
        public Task<bool> Register(RegisterRequest request);
    }
}

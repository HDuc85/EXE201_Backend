using System.Security.Claims;

namespace Service.Helper.Header
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
        }

        public static string GetRoleName(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}

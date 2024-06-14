using Data.Models;
using Microsoft.AspNetCore.Identity;
using Service.Interface;
using System.Net;
using System.Security.Claims;
using Service.Helper.Header;

namespace Exe201_backend.Middleware
{
    public class BannedUserMiddleware
    {
        private readonly RequestDelegate _next;

        public BannedUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, UserManager<User> userManager, IUserService userService)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    var isban = await userService.CheckUserBan(user.Id);

                    if (isban)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        await context.Response.WriteAsync("You are banned.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}

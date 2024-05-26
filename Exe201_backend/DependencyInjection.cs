
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Service.Interface;

namespace Exe201_backend
{
    public static class DependencyInjection
    {

        
     
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {

            var connectionString = GetConnectionString();
            services.AddDbContext<PostgresContext>(options => options.UseNpgsql(connectionString));
            return services;
        }

        private static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();
            var strConn = config["ConnectionStrings:DefaultConnection"];

            return strConn;
        }

        public static void AddTokenBearer( this IServiceCollection services)
        {
            IConfigurationRoot Configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", true, true)
                      .Build();

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"])),
                    ClockSkew = TimeSpan.Zero,
                };
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        var tokenHandler = context.HttpContext.RequestServices.GetRequiredService<ITokenHandler>();
                        return tokenHandler.ValidateToken(context);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        return Task.CompletedTask;
                    },
                };
            });

        }

    }
}

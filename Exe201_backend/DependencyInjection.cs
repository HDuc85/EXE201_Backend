
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Service.Interface;
using Microsoft.AspNetCore.Identity;
using Data.ViewModel.Helper;

namespace Exe201_backend
{
    public static class DependencyInjection
    {

        private static readonly IConfiguration configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", true, true)
                      .Build();

        public static void AddDatabase(this IServiceCollection services)
        {

            var connectionString = GetConnectionString();
            services.AddDbContext<PostgresContext>(options => options.UseNpgsql(connectionString));

         
            
        }

        public static void AddFirebaseConfig(this IServiceCollection services)
        {
            services.Configure<FirebaseConfigModel>(configuration.GetSection("Firebase"));

        }

        public static void AddEmailConfig (this IServiceCollection services)
        {
           
            services.Configure<EmailConfig>(configuration.GetSection("Mail"));

        }

        private static string GetConnectionString()
        {
            
            var strConn = configuration["ConnectionStrings:DefaultConnection"];

            return strConn;
        }

        public static void AddTokenBearer( this IServiceCollection services)
        {
           
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.SaveToken = true;
                options.UseSecurityTokenValidators = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
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

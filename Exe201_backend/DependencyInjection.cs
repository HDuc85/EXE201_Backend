
using Microsoft.EntityFrameworkCore;
using Service.Models;

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
    }
}

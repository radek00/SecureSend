using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureSend.Infrastructure.EF.Context;
using SecureSend.Infrastructure.EF.Options;

namespace SecureSend.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var options = configuration.GetSection("SqlServer").Get<SqlServerOptions>();
            services.AddDbContext<SecureSendDbContext>(ctx =>
                ctx.UseSqlServer(options!.ConnectionString));

            return services;

        }
    }
}

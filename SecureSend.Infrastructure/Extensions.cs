using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureSend.Application.Services;
using SecureSend.Domain.Repositories;
using SecureSend.Infrastructure.EF.Context;
using SecureSend.Infrastructure.EF.Options;
using SecureSend.Infrastructure.Repositories;
using SecureSend.Infrastructure.Services;

namespace SecureSend.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISecureSendUploadRepository, SecureSendRepository>();

            services.AddScoped<IFileService, FileService>();

            var options = configuration.GetSection("SqlServer").Get<SqlServerOptions>();
            services.AddDbContext<SecureSendDbContext>(ctx =>
                ctx.UseSqlServer(options!.ConnectionString));

            return services;

        }
    }
}

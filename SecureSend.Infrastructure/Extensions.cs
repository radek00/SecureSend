using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureSend.Application.Services;
using SecureSend.Domain.Repositories;
using SecureSend.Infrastructure.BackgroundTasks;
using SecureSend.Infrastructure.EF.Context;
using SecureSend.Infrastructure.EF.Options;
using SecureSend.Infrastructure.HostedServices;
using SecureSend.Infrastructure.Middlewares;
using SecureSend.Infrastructure.Repositories;
using SecureSend.Infrastructure.Services;

namespace SecureSend.Infrastructure
{
    internal enum Databases
    {
        SqlServer,
        Postgres
    }
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISecureSendUploadRepository, SecureSendRepository>();

            services.AddScoped<IFileService, FileService>();

            services.AddScoped<ISecureUploadReadService, SecureUploadReadService>();

            services.AddScoped<ExceptionMiddleware>();

            services.AddSingleton<IUploadSizeTrackerService, UploadSizeTrackerService>();

            var database = configuration.GetSection(("Database")).Get<Databases>();

            if (database == Databases.SqlServer)
            {
                var options = configuration.GetSection("SqlServerOptions").Get<SqlServerOptions>();
                var sqlServerConnectionString = $"Server={options!.Server},{options!.Port};Database={options!.Database};Trusted_Connection={options!.TrustedConnection};User ID={options!.UserId};Password={options!.Password};TrustServerCertificate={options!.TrustServerCertificate}";
                services.AddDbContext<SecureSendDbWriteContext>(ctx =>
                    ctx.UseSqlServer(sqlServerConnectionString,
                        x => x.MigrationsAssembly("SecureSend.SqlServerMigrations")));

                services.AddDbContext<SecureSendDbReadContext>(ctx => ctx.UseSqlServer(sqlServerConnectionString,
                    x => x.MigrationsAssembly("SecureSend.SqlServerMigrations")));
                
            }
            else if (database == Databases.Postgres)
            {
                var options = configuration.GetSection("PostgresOptions").Get<PostgresOptions>();
                var connectionString =
                    $"User ID={options!.UserId};Password={options.Password};Host={options.Host};Port={options.Port};Database={options.Database}";
                services.AddDbContext<SecureSendDbWriteContext>(ctx =>
                    ctx.UseNpgsql(connectionString, x => x.MigrationsAssembly("SecureSend.PostgresMigrations")));
                services.AddDbContext<SecureSendDbReadContext>(ctx =>
                    ctx.UseNpgsql(connectionString, x => x.MigrationsAssembly("SecureSend.PostgresMigrations")));
            }
            

            services.AddHostedService<AppInitializer>();
            services.AddHostedService<BackgroundFileService>();
            services.AddHostedService<BackgroundFailedUploadRemoverService>();
            return services;

        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            return app;
        }
    }
}

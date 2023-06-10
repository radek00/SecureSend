﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureSend.Application.Services;
using SecureSend.Domain.Repositories;
using SecureSend.Infrastructure.BackgroundTasks;
using SecureSend.Infrastructure.EF.Context;
using SecureSend.Infrastructure.EF.Options;
using SecureSend.Infrastructure.Middlewares;
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

            services.AddScoped<ISecureUploadReadService, SecureUploadReadService>();

            services.AddScoped<ExceptionMiddleware>();

            var options = configuration.GetSection("SqlServerOptions").Get<SqlServerOptions>();
            services.AddDbContext<SecureSendDbWriteContext>(ctx =>
                ctx.UseSqlServer(options!.ConnectionString));

            services.AddDbContext<SecureSendDbReadContext>(ctx => ctx.UseSqlServer(options!.ConnectionString));

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

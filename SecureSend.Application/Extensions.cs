using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SecureSend.Application.Behaviors;
using SecureSend.Domain.Factories;
using System.Reflection;

namespace SecureSend.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

            services.AddSingleton<ISecureSendUploadFactory, SecureSendUploadFactory>();


            return services;
        }
    }
}

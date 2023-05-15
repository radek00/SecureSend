using Microsoft.Extensions.DependencyInjection;
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

            services.AddSingleton<ISecureSendUploadFactory, SecureSendUploadFactory>();


            return services;
        }
    }
}

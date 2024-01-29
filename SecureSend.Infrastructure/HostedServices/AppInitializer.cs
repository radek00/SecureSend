using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecureSend.Application.Services;

namespace SecureSend.Infrastructure.HostedServices;

internal sealed class AppInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AppInitializer(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {

        using var scope = _serviceProvider.CreateScope();
        await MigrateAsync(scope, cancellationToken);
        SetupCurrentUploadDirectorySize(scope, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(a => typeof(DbContext).IsAssignableFrom(a) && !a.IsInterface && a != typeof(DbContext));
        foreach (var dbContextType in dbContextTypes)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
            if (dbContext is null)
            {
                continue;
            }

            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    private void SetupCurrentUploadDirectorySize(IServiceScope scope, CancellationToken cancellationToken)
    {
        var sizeTrackerService = scope.ServiceProvider.GetRequiredService<IUploadSizeTrackerService>();
        sizeTrackerService.Setup();
    }
}
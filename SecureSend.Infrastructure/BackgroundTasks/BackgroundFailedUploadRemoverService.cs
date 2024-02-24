using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureSend.Application.Services;
using SecureSend.Infrastructure.EF.Context;
using SecureSend.Application.Options;

namespace SecureSend.Infrastructure.BackgroundTasks
{
    internal sealed class BackgroundFailedUploadRemoverService : BackgroundService
    {
        private readonly ILogger<BackgroundFileService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly FileStorageOptions _storage;

        public BackgroundFailedUploadRemoverService(ILogger<BackgroundFileService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _storage = new();
            _logger = logger;
            _serviceProvider = serviceProvider;
            configuration.GetSection("FileStoragePath").Bind(_storage);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                await RemoveFailedUploads(scope, stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }

        private async Task RemoveFailedUploads(IServiceScope scope, CancellationToken token)
        {

                _logger.LogInformation("Removing failed uploads: {@date}", DateTime.UtcNow);
                var dbContext = scope.ServiceProvider.GetRequiredService<SecureSendDbWriteContext>();
                var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                var query = dbContext.SecureSendUploads.Where(u => !u.Files.Any() && u.UploadDate <= DateTime.UtcNow.AddHours(-1));
                var emptyUploads = await query.AsNoTracking().ToListAsync(token);
                
                var trackerService = scope.ServiceProvider.GetRequiredService<IUploadSizeTrackerService>();
                foreach (var upload in emptyUploads)
                {
                    _logger.LogInformation("Removing failed upload: {@id}", upload.Id);
                    fileService.RemoveUpload(upload.Id);
                    trackerService.Remove(upload.Id);

                }

                if (emptyUploads.Any()) await query.ExecuteDeleteAsync(token);
                
            
        }
    }
}

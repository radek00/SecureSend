using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureSend.Application.Services;
using SecureSend.Infrastructure.EF.Context;
using SecureSend.Infrastructure.EF.Options;

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
            await RemoveFailedUploads(stoppingToken);
        }

        private async Task RemoveFailedUploads(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation("Removing failed uploads");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SecureSendDbWriteContext>();
                    var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                    var query = dbContext.SecureSendUploads.Where(u => !u.Files.Any());
                    var emptyUploads = await query.AsNoTracking().ToListAsync();
                    foreach (var upload in emptyUploads)
                    {
                        fileService.RemoveUpload(upload.Id);
                        _logger.LogInformation("Removing failed upload: {@id}", upload.Id);

                    }

                    if (emptyUploads.Any()) await query.ExecuteDeleteAsync();
                    
                    await Task.Delay(TimeSpan.FromMinutes(60), token);
                }
            }
        }
    }
}

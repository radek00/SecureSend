using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureSend.Application.Services;
using SecureSend.Infrastructure.EF.Context;

namespace SecureSend.Infrastructure.BackgroundTasks
{
    internal sealed class BackgroundFileService: BackgroundService
    {
        private readonly ILogger<BackgroundFileService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundFileService(ILogger<BackgroundFileService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RemoveExpiredUploads(stoppingToken);
        }

        private async Task RemoveExpiredUploads(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for expired files: {@date}", DateTime.UtcNow);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SecureSendDbWriteContext>();
                    var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                    var query = dbContext.SecureSendUploads.Where(u => u.ExpiryDate < DateTime.UtcNow);
                    var expiredUploads = await query.AsNoTracking().ToListAsync(token);

                    foreach (var upload in expiredUploads)
                    {
                        fileService.RemoveUpload(upload.Id);
                        _logger.LogInformation("Removing expired upload: {@id}", upload.Id);
                    }

                    if (expiredUploads.Count > 0) await query.ExecuteDeleteAsync(token);

                    await Task.Delay(TimeSpan.FromMinutes(30), token);
                }
            }

        }
    }
}

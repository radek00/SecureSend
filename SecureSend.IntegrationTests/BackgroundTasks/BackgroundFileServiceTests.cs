using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;
using SecureSend.Infrastructure.BackgroundTasks;

namespace SecureSend.IntegrationTests.BackgroundTasks;
public class BackgroundFileServiceTests(SecureSendWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RemoveExpiredUploads_ShouldRemoveExpiredUploads()
    {
        // Arrange
        var dbContext = base.DbWriteContext;

        var expiredUpload = new SecureSendUpload(
            new SecureSendUploadId(Guid.NewGuid()),
            new SecureSendUploadDate(DateTime.UtcNow.AddDays(-2)),
            new SecureSendExpiryDate(DateTime.UtcNow.AddDays(-1)), null);
            
        var activeUpload = new SecureSendUpload(
            new SecureSendUploadId(Guid.NewGuid()),
            new SecureSendUploadDate(DateTime.UtcNow),
            new SecureSendExpiryDate(DateTime.UtcNow.AddDays(1)), null);

        dbContext.SecureSendUploads.Add(expiredUpload);
        dbContext.SecureSendUploads.Add(activeUpload);
        await dbContext.SaveChangesAsync();

        var serviceProvider = base._scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<BackgroundFileService>>();

        var sut = new BackgroundFileService(logger, serviceProvider);

        // Act
        using var cts = new CancellationTokenSource();
        var task = sut.StartAsync(cts.Token);
        
        await Task.Delay(1000);
        cts.Cancel();
        
        try 
        { 
            await task; 
        } 
        catch (TaskCanceledException) {}

        // Assert
        var readContext = base.DbReadContext;
        
        var remainingUploads = await readContext.SecureSendUploads.ToListAsync();
        
        Assert.Contains(remainingUploads, u => u.Id == activeUpload.Id.Value);
        Assert.DoesNotContain(remainingUploads, u => u.Id == expiredUpload.Id.Value);
    }
}

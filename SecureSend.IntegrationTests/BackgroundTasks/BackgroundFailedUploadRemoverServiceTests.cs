using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;
using SecureSend.Infrastructure.BackgroundTasks;
using SecureSend.Infrastructure.EF.Context;
using Xunit;

namespace SecureSend.IntegrationTests.BackgroundTasks;

public class BackgroundFailedUploadRemoverServiceTests(SecureSendWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RemoveFailedUploads_ShouldRemoveUploads_WithoutFiles_And_OlderThanOneHour()
    {
        // Arrange
        var dbContext = base.DbWriteContext;

        var failedUpload = new SecureSendUpload(
            new SecureSendUploadId(Guid.NewGuid()),
            new SecureSendUploadDate(DateTime.UtcNow.AddHours(-2)),
            new SecureSendExpiryDate(DateTime.UtcNow.AddDays(1)), null);

        var activeUpload = new SecureSendUpload(
            new SecureSendUploadId(Guid.NewGuid()),
            new SecureSendUploadDate(DateTime.UtcNow.AddMinutes(-30)),
            new SecureSendExpiryDate(DateTime.UtcNow.AddDays(1)), null);

        var uploadWithFile = new SecureSendUpload(
            new SecureSendUploadId(Guid.NewGuid()),
            new SecureSendUploadDate(DateTime.UtcNow.AddHours(-2)),
            new SecureSendExpiryDate(DateTime.UtcNow.AddDays(1)), null);
            
        uploadWithFile.AddFile(new SecureSendFile("test.txt", "metadata"));

        dbContext.SecureSendUploads.Add(failedUpload);
        dbContext.SecureSendUploads.Add(activeUpload);
        dbContext.SecureSendUploads.Add(uploadWithFile);
        await dbContext.SaveChangesAsync();

        var serviceProvider = base._scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<BackgroundFileService>>();
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        
        var service = new BackgroundFailedUploadRemoverService(logger, serviceProvider, config);

        // Act
        using var cts = new CancellationTokenSource();
        var task = service.StartAsync(cts.Token);
        
        await Task.Delay(1000);
        cts.Cancel();
        
        try 
        { 
            await task; 
        } 
        catch (TaskCanceledException) {}

        // Assert
        var readDbcontext = base.DbReadContext;
        
        var remainingUploads = await readDbcontext.SecureSendUploads.ToListAsync();
        
        Assert.Contains(remainingUploads, u => u.Id == activeUpload.Id.Value);
        Assert.Contains(remainingUploads, u => u.Id == uploadWithFile.Id.Value);
        Assert.DoesNotContain(remainingUploads, u => u.Id == failedUpload.Id.Value);
    }
}

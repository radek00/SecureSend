using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureSend.Infrastructure.EF.Context;

namespace SecureSend.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<SecureSendWebApplicationFactory>, IDisposable
{
    protected readonly SecureSendWebApplicationFactory _factory;
    protected readonly IServiceScope _scope;

    protected SecureSendDbReadContext DbReadContext => _scope.ServiceProvider.GetRequiredService<SecureSendDbReadContext>();
    protected SecureSendDbWriteContext DbWriteContext => _scope.ServiceProvider.GetRequiredService<SecureSendDbWriteContext>();

    protected BaseIntegrationTest(SecureSendWebApplicationFactory factory)
    {
        _factory = factory;
        _scope = _factory.Services.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }
}

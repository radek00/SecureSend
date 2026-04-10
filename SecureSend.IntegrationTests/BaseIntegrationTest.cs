using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureSend.Infrastructure.EF.Context;

namespace SecureSend.IntegrationTests;

public class BaseIntegrationTest(SecureSendWebApplicationFactory factory) : IClassFixture<SecureSendWebApplicationFactory>
{
    protected readonly SecureSendWebApplicationFactory _factory = factory;
    protected SecureSendDbReadContext DbReadContext => GetContext<SecureSendDbReadContext>();
    protected SecureSendDbWriteContext DbWriteContext => GetContext<SecureSendDbWriteContext>();
    protected IServiceScope ServiceScope => _factory.Services.CreateScope();

    private TContext GetContext<TContext>() where TContext : DbContext
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TContext>();
    }
}

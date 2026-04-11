using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace SecureSend.IntegrationTests;

public class SecureSendWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    public string TempStoragePath { get; }

    public SecureSendWebApplicationFactory()
    {
        TempStoragePath = Path.Combine(Path.GetTempPath(), "SecureSendTests", Guid.NewGuid().ToString());
        _postgreSqlContainer = new PostgreSqlBuilder("postgres:18.3")
            .WithDatabase("SecureSend")
            .WithUsername("postgres")
            .WithPassword("example")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");
        builder.UseSetting("PostgresOptions:Host", _postgreSqlContainer.Hostname);
        builder.UseSetting("PostgresOptions:Port", _postgreSqlContainer.GetMappedPublicPort(5432).ToString());
        builder.UseSetting("PostgresOptions:Database", "SecureSend");
        builder.UseSetting("PostgresOptions:UserId", "postgres");
        builder.UseSetting("PostgresOptions:Password", "example");
        builder.UseSetting("FileStorageOptions:Path", TempStoragePath);
    }

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(TempStoragePath);
        await _postgreSqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        if (Directory.Exists(TempStoragePath))
        {
            Directory.Delete(TempStoragePath, true);
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace SecureSend.IntegrationTests;

public class SecureSendWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public SecureSendWebApplicationFactory()
    {
        _postgreSqlContainer = new PostgreSqlBuilder("postgres:18.3")
            .WithDatabase("SecureSend")
            .WithUsername("postgres")
            .WithPassword("example")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        Environment.SetEnvironmentVariable("PostgresOptions__Host", _postgreSqlContainer.Hostname);
        Environment.SetEnvironmentVariable("PostgresOptions__Port", _postgreSqlContainer.GetMappedPublicPort(5432).ToString());
        Environment.SetEnvironmentVariable("PostgresOptions__Database", "SecureSend");
        Environment.SetEnvironmentVariable("PostgresOptions__UserId", "postgres");
        Environment.SetEnvironmentVariable("PostgresOptions__Password", "example");
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }
}

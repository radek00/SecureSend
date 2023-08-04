namespace SecureSend.Infrastructure.EF.Options;

public class PostgresOptions
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string UserId { get; set; }
    public string Password{ get; set; }
}
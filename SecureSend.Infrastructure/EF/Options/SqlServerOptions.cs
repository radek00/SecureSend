namespace SecureSend.Infrastructure.EF.Options
{
    public class SqlServerOptions
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string TrustedConnection { get; set; }
        public string UserId { get; set; }
        public string Password{ get; set; }
        public string TrustServerCertificate { get; set; }

    }
}

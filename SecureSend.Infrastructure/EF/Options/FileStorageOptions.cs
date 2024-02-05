namespace SecureSend.Infrastructure.EF.Options
{
    public class FileStorageOptions
    {
        public string Path { get; init; }
        public double SingleUploadLimitInGB { get; init; }
        public double TotalUploadLimitInGB { get; init; }
    }
}

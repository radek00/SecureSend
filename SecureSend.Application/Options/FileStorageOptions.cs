namespace SecureSend.Application.Options
{
    public class FileStorageOptions
    {
        public string Path { get; init; } = null!;
        public double SingleUploadLimitInGB { get; init; } = 0;
        public double TotalUploadLimitInGB { get; init; } = 0;
        public int MaxExpirationInDays { get; set; } = 0;
    }
}

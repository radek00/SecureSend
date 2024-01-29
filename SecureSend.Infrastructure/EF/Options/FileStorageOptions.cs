namespace SecureSend.Infrastructure.EF.Options
{
    public class FileStorageOptions
    {
        public string Path { get; init; }
        public double SingleUploadLimit { get; init; }
        public double TotalUploadLimit { get; init; }
    }
}

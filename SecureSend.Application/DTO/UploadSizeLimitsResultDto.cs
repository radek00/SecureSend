namespace SecureSend.Application.DTO;

public class UploadSizeLimitsResultDto
{
    public double SingleUploadLimitInGB { get; set; }
    public int MaxExpirationInDays { get; set; }
}
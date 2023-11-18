namespace SecureSend.Application.DTO;

public class UploadVerifyResponseDTO
{
    public Guid SecureUploadId { get; set; }
    public bool IsProtected { get; set; }
}
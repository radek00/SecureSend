
namespace SecureSend.Application.Commands
{
    public record CreateSecureUpload(Guid uploadId, DateTime? expiryDate): ICommand;
}

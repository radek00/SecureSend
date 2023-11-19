using SecureSend.Application.DTO;

namespace SecureSend.Application.Commands
{
    public record ViewSecureUpload(Guid id, string? password) : ICommand<SecureUploadDto>;
}

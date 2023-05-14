

namespace SecureSend.Application.Commands
{
    public record CreateSecureUpload(Guid uploadId, int chunkNumber, int totalChunks, DateTime expiryDate, string fileName): ICommand;
}

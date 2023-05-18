

using Microsoft.AspNetCore.Http;

namespace SecureSend.Application.Commands
{
    public record CreateSecureUpload(Guid uploadId, int chunkNumber, int totalChunks, DateTime expiryDate, IFormFile chunk): ICommand;
}

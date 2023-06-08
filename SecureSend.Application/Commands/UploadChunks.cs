using MediatR;
using Microsoft.AspNetCore.Http;

namespace SecureSend.Application.Commands
{
    public record UploadChunks(Guid uploadId, int chunkNumber, int totalChunks, IFormFile chunk): ICommand<Unit>
    {
    }
}

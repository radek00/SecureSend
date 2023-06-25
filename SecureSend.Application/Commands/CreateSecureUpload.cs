
using MediatR;

namespace SecureSend.Application.Commands
{
    public record CreateSecureUpload(Guid uploadId, DateTime? expiryDate, string password): ICommand<Unit>;
}

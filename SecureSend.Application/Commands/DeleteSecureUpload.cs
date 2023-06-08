using MediatR;

namespace SecureSend.Application.Commands
{
    public record DeleteSecureUpload(Guid id): ICommand<Unit>
    {
    }
}

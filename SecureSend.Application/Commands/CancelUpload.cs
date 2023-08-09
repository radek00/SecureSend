using MediatR;

namespace SecureSend.Application.Commands.Handlers;

public record CancelUpload(Guid id, string fileName): ICommand<Unit>;
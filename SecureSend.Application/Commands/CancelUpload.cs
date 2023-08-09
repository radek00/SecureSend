using MediatR;

namespace SecureSend.Application.Commands;

public record CancelUpload(Guid id, string fileName): ICommand<Unit>;
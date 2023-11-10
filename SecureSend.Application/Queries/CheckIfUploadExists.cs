using MediatR;
using SecureSend.Application.Queries;

public record CheckIfUploadExists(Guid id) : IQuery<bool>
{
}
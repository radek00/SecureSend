using MediatR;

namespace SecureSend.Application.Commands
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<T> : IRequest<T>
    {
    }
}

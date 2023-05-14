

using MediatR;

namespace SecureSend.Application.Commands.Handlers
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand> where TCommand: class, ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : class, ICommand<TResult>
    {
    }
}

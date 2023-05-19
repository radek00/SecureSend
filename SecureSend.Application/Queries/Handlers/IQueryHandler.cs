using MediatR;

namespace SecureSend.Application.Queries.Handlers
{
    public interface IQueryHandler<TQuery, TResult>: IRequestHandler<TQuery, TResult> where TQuery : class, IQuery<TResult>
    {
    }
}


using MediatR;

namespace SecureSend.Application.Queries
{
    public interface IQuery<TResponse>: IRequest<TResponse>
    {
    }
}

using MediatR;
using Microsoft.Extensions.Logging;

namespace SecureSend.Application.Behaviors
{
    internal sealed class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse> where TResponse : class
    {
        private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

        public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);

            try
            {
                var result = await next();
                _logger.LogInformation("Completed request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Request failure {@RequestName}, {@Message}, {@DateTimeUtc}", typeof(TRequest).Name, ex.Message, DateTime.UtcNow);
                throw;
            }

        }
    }
}

using MediatR;
using SecureSend.Domain.Events;
using SecureSend.Domain.Repositories;

namespace SecureSend.Application.EventHandlers
{
    internal sealed class SecureUploadViewedHandler : INotificationHandler<SecureUploadViewedEvent>
    {
        private readonly ISecureSendUploadRepository _secureSendUploadRepository;

        public SecureUploadViewedHandler(ISecureSendUploadRepository secureSendUploadRepository)
        {
            _secureSendUploadRepository = secureSendUploadRepository;
        }

        public async Task Handle(SecureUploadViewedEvent notification, CancellationToken cancellationToken)
        {
            await _secureSendUploadRepository.SaveChanges(cancellationToken);
        }
    }
}

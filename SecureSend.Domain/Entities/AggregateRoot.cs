using SecureSend.Domain.Events;

namespace SecureSend.Domain.Entities
{
    public abstract class AggregateRoot<T>
    {
        public T Id { get; protected set; }

        private readonly List<IDomainEvent> _events = new();

        protected AggregateRoot(T id)
        {
            Id = id;
        }

        protected AggregateRoot() { }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _events.Add(domainEvent);
        }
    }
}

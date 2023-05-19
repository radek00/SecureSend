namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendExpiryDate
    {
        public DateTime? Value { get; private set; }

        public SecureSendExpiryDate(DateTime? value = null)
        {
            Value = value is not null ? value?.ToUniversalTime() : null;
        }


        public static implicit operator DateTime?(SecureSendExpiryDate secureSendExpiryDate) => secureSendExpiryDate?.Value;

        public static implicit operator SecureSendExpiryDate(DateTime? secureSendExpiryDate) => new(secureSendExpiryDate);
    }
}

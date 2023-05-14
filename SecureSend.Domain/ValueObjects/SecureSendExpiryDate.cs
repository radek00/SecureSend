namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendExpiryDate
    {
        public DateTime? Value { get; set; }

        public SecureSendExpiryDate(DateTime? value)
        {
            Value = value is not null ? value?.ToUniversalTime() : null;
        }


        public static implicit operator DateTime?(SecureSendExpiryDate secureSendExpiryDate) => secureSendExpiryDate.Value;

        public static implicit operator SecureSendExpiryDate(DateTime? secureSendExpiryDate) => new(secureSendExpiryDate);
    }
}

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendExpiryDate
    {
        public DateTime? Value { get; private set; }

        public SecureSendExpiryDate(DateTime? value)
        {
            Value = value;
        }

        public static SecureSendExpiryDate Create(DateTime? value = null)
        {
            return new SecureSendExpiryDate(value?.Date);
        }


        public static implicit operator DateTime?(SecureSendExpiryDate? secureSendExpiryDate) => secureSendExpiryDate?.Value;

        public static implicit operator SecureSendExpiryDate(DateTime? secureSendExpiryDate) => new(secureSendExpiryDate);
    }
}

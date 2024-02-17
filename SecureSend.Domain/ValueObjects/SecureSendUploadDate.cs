namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendUploadDate
    {
        public DateTime Value { get; private set; }

        public SecureSendUploadDate(DateTime date)
        {
            Value = date;
        }
        
        public static SecureSendUploadDate Create()
        {
            return new SecureSendUploadDate(DateTime.UtcNow);
        }

        public static implicit operator DateTime(SecureSendUploadDate secureSendUploadDate) => secureSendUploadDate.Value;

        public static implicit operator SecureSendUploadDate(DateTime secureSendUploadDate) => new(secureSendUploadDate);

    }
}

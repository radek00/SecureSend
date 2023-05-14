namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendUploadDate
    {
        public DateTime Value { get; set; }

        public SecureSendUploadDate()
        {
            Value = DateTime.UtcNow;
        }

        public static implicit operator DateTime(SecureSendUploadDate secureSendUploadDate) => secureSendUploadDate.Value;

        //public static implicit operator SecureSendUploadDate(DateTime secureSendUploadDate) => new();

    }
}

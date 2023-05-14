namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendIsViewed
    {
        public bool Value { get; set; }

        public SecureSendIsViewed(bool value)
        {
            Value = value;
        }

        public static implicit operator bool(SecureSendIsViewed value) => value.Value;

        public static implicit operator SecureSendIsViewed(bool isviewed) => new(isviewed);
    }
}

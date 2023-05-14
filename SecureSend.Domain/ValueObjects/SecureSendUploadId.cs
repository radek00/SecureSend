using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendUploadId
    {
        public Guid Value { get; set; }

        public SecureSendUploadId(Guid value)
        {
            if (value == Guid.Empty) throw new EmptySecureSendUploadIdException();
            Value = value;
        }

        public static implicit operator Guid(SecureSendUploadId value) => value.Value;

        public static implicit operator SecureSendUploadId(Guid id) => new(id);
    }
}

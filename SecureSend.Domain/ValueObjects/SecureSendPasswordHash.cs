using SecureSend.Domain.Services;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendPasswordHash
    {
        public string Value { get; private set; }

        public SecureSendPasswordHash(string value)
        {
            Value = SecretHasher.Hash(value);
        }

        public static implicit operator string(SecureSendPasswordHash password) => password.Value;

        public static implicit operator SecureSendPasswordHash(string password) => new(password);
    }
}

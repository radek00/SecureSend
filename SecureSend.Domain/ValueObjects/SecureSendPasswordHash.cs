using SecureSend.Domain.Services;

namespace SecureSend.Domain.Entities;

public class SecureSendPasswordHash
{
    public byte[]? Value { get;}

    public SecureSendPasswordHash(byte[]? hash)
    {
        Value = hash;
    }

    public void VerifyHash(string? password)
    {
        if (Value is null || String.IsNullOrEmpty(password) || !new HashingService().Verify(password, Value)) throw new InvalidPasswordException();
    }

    public static SecureSendPasswordHash Create(string? password)
    {
        var value  = String.IsNullOrEmpty(password) ? null : new HashingService().Hash(password);
        return new SecureSendPasswordHash(value);
    }
    
    public static implicit operator byte[]?(SecureSendPasswordHash value) => value.Value;

    public static implicit operator SecureSendPasswordHash(byte[]? hash) => new(hash);
}
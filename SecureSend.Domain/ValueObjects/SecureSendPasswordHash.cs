using SecureSend.Domain.Services;

namespace SecureSend.Domain.Entities;

public class SecureSendPasswordHash
{
    public byte[]? Value { get;}
    private readonly  HashingService _hashingService;
    public SecureSendPasswordHash(string? password)
    {
        _hashingService = new HashingService();
        Value = String.IsNullOrEmpty(password) ? null : _hashingService.Hash(password);
    }

    public SecureSendPasswordHash(byte[]? hash)
    {
        _hashingService = new HashingService();
        Value = hash;
    }

    public void VerifyHash(string password)
    {
        if (Value is null || !_hashingService.Verify(password, Value)) throw new InvalidPasswordException();
    }
    
    public static implicit operator byte[]?(SecureSendPasswordHash value) => value.Value;

    public static implicit operator SecureSendPasswordHash(byte[] hash) => new(hash);

    public static implicit operator SecureSendPasswordHash(string password) => new(password);
}
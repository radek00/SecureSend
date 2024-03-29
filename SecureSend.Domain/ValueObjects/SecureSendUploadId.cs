﻿using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendUploadId
    {
        public Guid Value { get; private set; }

        public SecureSendUploadId(Guid value)
        {
            Value = value;
        }

        public static SecureSendUploadId Create(Guid value)
        {
            if (value == Guid.Empty) throw new EmptySecureSendUploadIdException();
            return new SecureSendUploadId(value);
        }

        public static implicit operator Guid(SecureSendUploadId value) => value.Value;

        public static implicit operator SecureSendUploadId(Guid id) => new(id);
    }
}

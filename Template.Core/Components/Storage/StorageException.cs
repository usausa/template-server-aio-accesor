namespace Template.Components.Storage;

using System;

public sealed class StorageException : Exception
{
    public StorageException()
    {
    }

    public StorageException(string message)
        : base(message)
    {
    }

    public StorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

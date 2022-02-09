namespace Template.Components.Storage;

using System.IO;

public interface IStorage
{
    ValueTask<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);

    ValueTask<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default);

    ValueTask<string[]> ListAsync(string path, CancellationToken cancellationToken = default);

    ValueTask DeleteAsync(string path, CancellationToken cancellationToken = default);

    ValueTask<Stream> ReadAsync(string path, CancellationToken cancellationToken = default);

    ValueTask WriteAsync(string path, Stream stream, CancellationToken cancellationToken = default);
}

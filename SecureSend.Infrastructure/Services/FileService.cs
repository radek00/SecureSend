using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureSend.Application.Services;
using SecureSend.Domain.ValueObjects;
using SecureSend.Application.Options;
using SecureSend.Infrastructure.Exceptions;
using System.Collections.Concurrent;
using SecureSend.Application.Exceptions;

namespace SecureSend.Infrastructure.Services
{

    internal sealed class FileService : IFileService
    {
        private readonly IOptions<FileStorageOptions> _fileStorageOptions;
        
        private class UploadState
        {
            public SemaphoreSlim Lock { get; } = new(1, 1);
            public int NextChunk { get; set; } = 1;
            public required SecureSendFile SecureSendFile { get; set; }
            public required int TotalChunks { get; set; }
        }

        private static readonly ConcurrentDictionary<string, UploadState> _uploadStates = new();

        public FileService(IOptions<FileStorageOptions> fileStorageOptions)
        {
            _fileStorageOptions = fileStorageOptions;
        }

        public FileStream? DownloadFile(Guid uploadId, string fileName)
        {
            var directory = GetUploadDirectory(uploadId);

            var file = directory?.GetFiles().FirstOrDefault(f => f.Name == fileName);

            return file != null ? new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true) : null;
        }

        public async Task<SecureSendFile?> HandleChunk(SecureUploadChunk chunk, Guid uploadId, long totalFileSize)
        {
            var state = _uploadStates.GetOrAdd(chunk.ChunkDirectory, _ => new UploadState
            {
                SecureSendFile = SecureSendFile.Create(chunk.Chunk.FileName, chunk.ContentType, totalFileSize),
                TotalChunks = chunk.TotalChunks
            });

            if (state.TotalChunks != chunk.TotalChunks)
            {
                throw new InvalidChunkCountException(chunk.TotalChunks, state.TotalChunks);
            }

            await state.Lock.WaitAsync();
            try
            {
                var uploadDir = GetUploadDirectory(uploadId);
                var chunkDir = GetChunkDirectory(uploadId, chunk.ChunkDirectory);
                var finalFilePath = Path.Combine(uploadDir!.FullName, state.SecureSendFile.RandomFileName);

                if (chunk.ChunkNumber == state.NextChunk)
                {
                    await AppendChunk(finalFilePath, chunk.Chunk);
                    state.NextChunk++;

                    while (state.NextChunk <= state.TotalChunks)
                    {
                        var nextChunkFile = chunkDir.GetFiles($"{state.NextChunk}_*").FirstOrDefault();
                        if (nextChunkFile == null) break;

                        await AppendFile(finalFilePath, nextChunkFile.FullName);
                        nextChunkFile.Delete();
                        state.NextChunk++;
                    }
                }
                else
                {
                    await SaveChunkFile(chunkDir, chunk);
                }

                if (state.NextChunk > state.TotalChunks)
                {
                    chunkDir.Delete(true);
                    _uploadStates.TryRemove(chunk.ChunkDirectory, out _);
                    return state.SecureSendFile;
                }

                return null;
            }
            finally
            {
                state.Lock.Release();
            }
        }

        private static async Task AppendChunk(string filePath, IFormFile chunk)
        {
            await using var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
            await chunk.CopyToAsync(stream);
        }

        private static async Task AppendFile(string filePath, string chunkFilePath)
        {
            await using var chunkStream = new FileStream(chunkFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
            await using var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
            await chunkStream.CopyToAsync(stream);
        }

        private static async Task SaveChunkFile(DirectoryInfo chunkDir, SecureUploadChunk chunk)
        {
            await using var output = System.IO.File.OpenWrite($"{chunkDir.FullName}/{chunk.ChunkName}");
            await chunk.Chunk.CopyToAsync(output);
        }

        public IEnumerable<string> GetChunksList(Guid uploadId, string chunkDirectory)
        {
            var directory = GetChunkDirectory(uploadId, chunkDirectory).FullName;
            var chunkFiles = Directory.GetFiles(directory)
                                      .Select(x => Path.GetFileName(x))
                                      .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('_')[0]))
                                      .ToList();
            return chunkFiles;
        }

        private DirectoryInfo GetChunkDirectory(Guid uploadId, string chunkDirectory)
        {
            var uploadDirectory = GetUploadDirectory(uploadId);
            if (uploadDirectory is null) throw new MissingUploadDirectoryException(uploadId);
            return Directory.CreateDirectory($"{_fileStorageOptions.Value.Path}/{uploadId}/{chunkDirectory}");
        }

        private DirectoryInfo? GetUploadDirectory(Guid uploadId)
        {
            if (Directory.Exists($"{_fileStorageOptions.Value.Path}/{uploadId}")) return Directory.CreateDirectory($"{_fileStorageOptions.Value.Path}/{uploadId}");
            return null;
        }

        public void RemoveUpload(Guid uploadId)
        {
            var directory = GetUploadDirectory(uploadId);

            if (directory != null) Directory.Delete(directory.FullName, true);
        }

        public void RemoveFileFromUpload(Guid uploadId, string fileName)
        {
            var fileDir = GetUploadDirectory(uploadId)?.GetDirectories()
                .FirstOrDefault(x => x.Name == Path.GetFileNameWithoutExtension(fileName));
            if (fileDir is not null) Directory.Delete(fileDir.FullName, true);
        }

        public double GetCurrentUploadDirectorySize()
        {
            var di = new DirectoryInfo(_fileStorageOptions.Value.Path);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }

        public void SetupUploadDirectory(Guid uploadId)
        {
            Directory.CreateDirectory($"{_fileStorageOptions.Value.Path}/{uploadId}");
        }
    }
}

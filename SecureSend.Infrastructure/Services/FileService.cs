using Microsoft.Extensions.Options;
using SecureSend.Application.Services;
using SecureSend.Domain.ValueObjects;
using SecureSend.Application.Options;
using SecureSend.Infrastructure.Exceptions;
using System.Collections.Concurrent;

namespace SecureSend.Infrastructure.Services
{

    internal sealed class FileService : IFileService
    {
        private readonly IOptions<FileStorageOptions> _fileStorageOptions;
        private static readonly ConcurrentDictionary<Guid, int> _uploadCounters = new();
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

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

        public async Task SaveChunkToDisk(SecureUploadChunk chunk, Guid uploadId)
        {
            var directory = GetChunkDirectory(uploadId, chunk.ChunkDirectory);
            await using var output = System.IO.File.OpenWrite($"{directory.FullName}/{chunk.ChunkName}");
            await chunk.Chunk.CopyToAsync(output);
        }

        public async Task<bool> MergeChunks(Guid uploadId, string chunkDirectory, int totalChunks)
        {
            var uploadLock = _locks.GetOrAdd(uploadId, _ => new SemaphoreSlim(1, 1));
            await uploadLock.WaitAsync();

            try
            {
                var dir = GetChunkDirectory(uploadId, chunkDirectory);
                var mergedFilePath = Path.Combine(dir.FullName, "merged");

                var nextChunk = _uploadCounters.GetOrAdd(uploadId, 1);

                while (nextChunk <= totalChunks)
                {
                    var chunkFile = dir.GetFiles($"{nextChunk}_*").FirstOrDefault();
                    if (chunkFile == null) break;

                    await using (var chunkStream = new FileStream(chunkFile.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                    await using (var mergedStream = new FileStream(mergedFilePath, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        await chunkStream.CopyToAsync(mergedStream);
                    }
                    chunkFile.Delete();
                    nextChunk++;
                }

                _uploadCounters[uploadId] = nextChunk;
                return nextChunk > totalChunks;
            }
            finally
            {
                uploadLock.Release();
            }
        }

        public Task FinalizeUpload(Guid uploadId, string chunkDirectory, string fileName)
        {
            var dir = GetChunkDirectory(uploadId, chunkDirectory);
            var mergedFilePath = Path.Combine(dir.FullName, "merged");
            var finalPath = Path.Combine(dir.Parent!.FullName, fileName);

            if (File.Exists(mergedFilePath))
            {
                File.Move(mergedFilePath, finalPath);
            }
            Directory.Delete(dir.FullName, true);

            _uploadCounters.TryRemove(uploadId, out _);
            _locks.TryRemove(uploadId, out _);

            return Task.CompletedTask;
        }

        public async Task MergeFiles(Guid uploadId, IEnumerable<string> chunkFiles, string chunkDirectory, string randomFileName)
        {
            
            var dir = GetChunkDirectory(uploadId, chunkDirectory);

            var mergedFilePath = Path.Combine(dir.Parent!.FullName, randomFileName);
            await using (var mergedFile = new FileStream(mergedFilePath, FileMode.Create))
            {
                foreach (var chunkFile in chunkFiles)
                {
                    await using var chunkStream = new FileStream(Path.Combine(dir.FullName, chunkFile), FileMode.Open);
                    await chunkStream.CopyToAsync(mergedFile);
                }
            }
            Directory.Delete(dir.FullName, true);
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

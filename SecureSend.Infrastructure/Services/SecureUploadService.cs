using Microsoft.Extensions.Configuration;
using SecureSend.Application.Services;
using SecureSend.Domain.ValueObjects;
using SecureSend.Infrastructure.EF.Options;

namespace SecureSend.Infrastructure.Services
{

    internal sealed class FileService : IFileService
    {
        private readonly FileStorageOptions _storage;

        public FileService(IConfiguration configuration)
        {
            _storage = new();
            configuration.GetSection("FileStoragePath").Bind(_storage);
        }

        public FileStream? DownloadFile(Guid uploadId, string fileName)
        {
            var directory = GetDirectory(uploadId);

            var file = directory?.GetFiles().FirstOrDefault(f => f.Name == fileName);

            if (file != null)
            {
                return new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            }

            return null;

        }

        public async Task SaveChunkToDisk(SecureUploadChunk chunk, Guid uploadId)
        {
            var directory = GetOrCreateDirectory(uploadId);
            using (FileStream output = System.IO.File.OpenWrite($"{directory.FullName}/{chunk.ChunkName}"))
            {
                await chunk.Chunk.CopyToAsync(output);
            }
        }

        public async Task MergeFiles(Guid uploadId, IEnumerable<string> chunkFiles)
        {


            var dir = GetOrCreateDirectory(uploadId);

            var chunkName = chunkFiles.FirstOrDefault();
            var fileName = chunkName!.Substring(chunkName.IndexOf("_") + 1);
            var mergedFilePath = Path.Combine(dir.Parent.FullName, fileName);
            using (var mergedFile = new FileStream(mergedFilePath, FileMode.Create))
            {
                foreach (var chunkFile in chunkFiles)
                {
                    using (var chunkStream = new FileStream(chunkFile, FileMode.Open))
                    {
                        await chunkStream.CopyToAsync(mergedFile);
                    }
                }
            }
            Directory.Delete(dir.FullName, true);

        }

        public IEnumerable<string> GetChunksList(Guid uploadId)
        {
            var chunkDirectory = GetOrCreateDirectory(uploadId).FullName;
            var chunkFiles = Directory.GetFiles(chunkDirectory)
                                      .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('_')[0]))
                                      .ToList();
            return chunkFiles;
        }

        private DirectoryInfo GetOrCreateDirectory(Guid uploadId)
        {
            return System.IO.Directory.CreateDirectory($"{_storage.Path}/{uploadId}/chunks");
        }

        private DirectoryInfo? GetDirectory(Guid uploadId)
        {
            if (Directory.Exists($"{_storage.Path}/{uploadId}")) return Directory.CreateDirectory($"{_storage.Path}/{uploadId}");
            return null;
        }

        public void RemovecancelledUpload(Guid uploadId)
        {
            var directory = GetDirectory(uploadId);

            if (directory != null) Directory.Delete(directory.FullName, true);
        }
    }
}

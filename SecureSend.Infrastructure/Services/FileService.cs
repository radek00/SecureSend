using Microsoft.Extensions.Options;
using SecureSend.Application.Services;
using SecureSend.Domain.ValueObjects;
using SecureSend.Infrastructure.EF.Options;

namespace SecureSend.Infrastructure.Services
{

    internal sealed class FileService : IFileService
    {
        private readonly IOptions<FileStorageOptions> _fileStorageOptions;

        public FileService(IOptions<FileStorageOptions> fileStorageOptions)
        {
            _fileStorageOptions = fileStorageOptions;
        }

        public FileStream? DownloadFile(Guid uploadId, string fileName)
        {
            var directory = GetDirectory(uploadId);

            var file = directory?.GetFiles().FirstOrDefault(f => f.Name == fileName);

            return file != null ? new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true) : null;
        }

        public async Task SaveChunkToDisk(SecureUploadChunk chunk, Guid uploadId)
        {
            var directory = GetOrCreateDirectory(uploadId, chunk.ChunkDirectory);
            await using var output = System.IO.File.OpenWrite($"{directory.FullName}/{chunk.ChunkName}");
            await chunk.Chunk.CopyToAsync(output);
        }

        public async Task MergeFiles(Guid uploadId, IEnumerable<string> chunkFiles, string chunkDirectory, string randomFileName)
        {


            var dir = GetOrCreateDirectory(uploadId, chunkDirectory);

            var mergedFilePath = Path.Combine(dir.Parent.FullName, randomFileName);
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
            var directory = GetOrCreateDirectory(uploadId, chunkDirectory).FullName;
            var chunkFiles = Directory.GetFiles(directory)
                                      .Select(x => Path.GetFileName(x))
                                      .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('_')[0]))
                                      .ToList();
            return chunkFiles;
        }

        private DirectoryInfo GetOrCreateDirectory(Guid uploadId, string chunkDirectory)
        {
            return System.IO.Directory.CreateDirectory($"{_fileStorageOptions.Value.Path}/{uploadId}/{chunkDirectory}");
        }

        private DirectoryInfo? GetDirectory(Guid uploadId)
        {
            if (Directory.Exists($"{_fileStorageOptions.Value.Path}/{uploadId}")) return Directory.CreateDirectory($"{_fileStorageOptions.Value.Path}/{uploadId}");
            return null;
        }

        public void RemoveUpload(Guid uploadId)
        {
            var directory = GetDirectory(uploadId);

            if (directory != null) Directory.Delete(directory.FullName, true);
        }

        public void RemoveFileFromUpload(Guid uploadId, string fileName)
        {
            var fileDir = GetDirectory(uploadId)?.GetDirectories()
                .FirstOrDefault(x => x.Name == Path.GetFileNameWithoutExtension(fileName));
            if (fileDir is not null) Directory.Delete(fileDir.FullName, true);
        }

        public double GetCurrentUploadDirectorySize()
        {
            var di = new DirectoryInfo(_fileStorageOptions.Value.Path);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }
    }
}

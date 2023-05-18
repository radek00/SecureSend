﻿using SecureSend.Application.Services;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Infrastructure.Services
{

    internal sealed class FileService : IFileService
    {
        public FileStream DownloadFile(Guid uploadId, string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task SaveChunkToDisk(SecureUploadChunk chunk, Guid uploadId)
        {
            var directory = GetDirectory(uploadId);
            using (FileStream output = System.IO.File.OpenWrite($"{directory.FullName}/{chunk.ChunkName}"))
            {
                await chunk.Chunk.CopyToAsync(output);
            }
        }

        public async Task MergeFiles(Guid uploadId, IEnumerable<string> chunkFiles)
        {


            var dir = GetDirectory(uploadId);

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
            var chunkDirectory = GetDirectory(uploadId).FullName;
            var chunkFiles = Directory.GetFiles(chunkDirectory)
                                      .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('_')[0]))
                                      .ToList();
            return chunkFiles;
        }

        private DirectoryInfo GetDirectory(Guid uploadId)
        {
            return System.IO.Directory.CreateDirectory($"./Files/{uploadId}/chunks");
        }
    }
}
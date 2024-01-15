﻿using Microsoft.AspNetCore.Http;
using SecureSend.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureUploadChunk
    {
        public int ChunkNumber { get; }

        public int TotalChunks { get; }

        public bool IsLast { get; }

        public string ContentType { get; }

        public string ChunkName { get; }

        public IFormFile Chunk { get; }

        public string ChunkDirectory { get; }

        public SecureUploadChunk(int chunkNumber, int totalChunks, IFormFile chunk)
        {
            if (string.IsNullOrEmpty(chunk.FileName)) throw new EmptyFileNameException();
            ChunkNumber = chunkNumber;
            TotalChunks = totalChunks;
            IsLast = chunkNumber == totalChunks ? true : false;
            Chunk = chunk;
            ContentType = chunk.ContentType;
            ChunkName = $"{chunkNumber}_{Path.GetRandomFileName()}";
            ChunkDirectory = Path.GetFileNameWithoutExtension(SanitizeDirectoryName(chunk.FileName));
        }

        private string SanitizeDirectoryName(string fileName)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            return fileName;
        }
    }
}

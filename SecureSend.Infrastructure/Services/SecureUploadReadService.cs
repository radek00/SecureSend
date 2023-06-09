using Microsoft.EntityFrameworkCore;
using SecureSend.Application.Services;
using SecureSend.Domain.Entities;
using SecureSend.Domain.ReadModels;
using SecureSend.Infrastructure.EF.Context;

namespace SecureSend.Infrastructure.Services
{
    internal sealed class SecureUploadReadService: ISecureUploadReadService
    {
        private readonly DbSet<UploadedFilesReadModel> _uploads;
        private readonly SecureSendDbReadContext _context;

        public SecureUploadReadService(SecureSendDbReadContext context)
        {
            _context = context;
            _uploads = _context.UploadedFiles;
        }

        public async Task<UploadedFilesReadModel?> GetUploadedFile(string fileName, Guid id)
        {
            return await _uploads
                        .Where(x => x.SecureSendUploadId == id && x.FileName == fileName)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
        }
    }
}

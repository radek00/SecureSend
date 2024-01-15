using Microsoft.EntityFrameworkCore;
using SecureSend.Application.Services;
using SecureSend.Domain.ReadModels;
using SecureSend.Infrastructure.EF.Context;

namespace SecureSend.Infrastructure.Services
{
    internal sealed class SecureUploadReadService: ISecureUploadReadService
    {
        private readonly DbSet<UploadedFilesReadModel> _files;
        private readonly DbSet<SecureUploadsReadModel> _uploads;
        private readonly SecureSendDbReadContext _context;

        public SecureUploadReadService(SecureSendDbReadContext context)
        {
            _context = context;
            _uploads = _context.SecureSendUploads;
            _files = _context.UploadedFiles;
        }

        public async Task<Guid?> GetUploadId(Guid id, CancellationToken cancellationToken)
        {
            return await _uploads.AsNoTracking().Where(x => x.Id == id).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<UploadedFilesReadModel?> GetUploadedFile(string fileName, Guid id, CancellationToken cancellationToken)
        {
            return await _files
                        .Where(x => x.SecureSendUploadId == id && x.DisplayFileName == fileName)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<SecureUploadsReadModel?> GetSecureUpload(Guid id, CancellationToken cancellationToken)
        {
            return await _uploads.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }


    }
}

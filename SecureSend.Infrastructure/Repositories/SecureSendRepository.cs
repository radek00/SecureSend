using Microsoft.EntityFrameworkCore;
using SecureSend.Application.Exceptions;
using SecureSend.Domain.Entities;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;
using SecureSend.Infrastructure.EF.Context;

namespace SecureSend.Infrastructure.Repositories
{
    internal sealed class SecureSendRepository: ISecureSendUploadRepository
    {
        private readonly DbSet<SecureSendUpload> _uploads;
        private readonly SecureSendDbWriteContext _context;

        public SecureSendRepository(SecureSendDbWriteContext context)
        {
            _context = context;
            _uploads = context.SecureSendUploads;
        }

        public async Task AddAsync(SecureSendUpload upload, CancellationToken cancellationToken)
        {
            await _uploads.AddAsync(upload, cancellationToken);
            await SaveChanges(cancellationToken);
        }

        public async Task DeleteAsync(SecureSendUpload upload, CancellationToken cancellationToken)
        {
            _context.Remove(upload);
            await SaveChanges(cancellationToken);
            
        }

        public async Task<SecureSendUpload?> GetAsync(SecureSendUploadId id, CancellationToken cancellationToken)
        {
            return  await _uploads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(SecureSendUpload upload, CancellationToken cancellationToken)
        {
            _uploads.Update(upload);
            await SaveChanges(cancellationToken);
        }

        public async Task SaveChanges(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

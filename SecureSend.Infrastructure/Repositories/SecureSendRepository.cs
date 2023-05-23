using Microsoft.EntityFrameworkCore;
using SecureSend.Domain.Entities;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;
using SecureSend.Infrastructure.EF.Context;

namespace SecureSend.Infrastructure.Repositories
{
    internal sealed class SecureSendRepository: ISecureSendUploadRepository
    {
        private readonly DbSet<SecureSendUpload> _uploads;
        private readonly SecureSendDbContext _context;

        public SecureSendRepository(SecureSendDbContext context)
        {
            _context = context;
            _uploads = context.SecureSendUploads;
        }

        public async Task AddAsync(SecureSendUpload upload)
        {
            await _uploads.AddAsync(upload);
            await SaveChanges();
        }

        public async Task DeleteAsync(SecureSendUpload upload)
        {
            _context.Remove(upload);
            await SaveChanges();
            
        }

        public async Task<SecureSendUpload> GetAsync(SecureSendUploadId id, bool track)
        {
            var query = track ? _uploads : _uploads.AsNoTracking();
            var upload = await query.FirstOrDefaultAsync(x => x.Id == id);
            return upload;
        }

        public async Task UpdateAsync(SecureSendUpload upload)
        {
            _uploads.Update(upload);
            await SaveChanges();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}

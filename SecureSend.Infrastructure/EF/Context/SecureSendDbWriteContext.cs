using Microsoft.EntityFrameworkCore;
using SecureSend.Domain.Entities;
using SecureSend.Infrastructure.EF.Config;

namespace SecureSend.Infrastructure.EF.Context
{
    public sealed class SecureSendDbWriteContext: DbContext
    {
        public DbSet<SecureSendUpload> SecureSendUploads { get; set; }

        public SecureSendDbWriteContext(DbContextOptions<SecureSendDbWriteContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("upload");
            var config = new SecureSendUploadWriteConfiguration();
            modelBuilder.ApplyConfiguration<SecureSendUpload>(config);
        }
    }
}

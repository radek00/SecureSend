using Microsoft.EntityFrameworkCore;
using SecureSend.Domain.Entities;
using SecureSend.Infrastructure.EF.Config;
using SecureSend.Domain.ReadModels;

namespace SecureSend.Infrastructure.EF.Context
{
    internal sealed class SecureSendDbReadContext: DbContext
    {
        public DbSet<SecureUploadsReadModel> SecureSendUploads { get; set; }
        public DbSet<UploadedFilesReadModel> UploadedFiles { get; set; }

        public SecureSendDbReadContext (DbContextOptions<SecureSendDbReadContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("upload");
            var config = new SecureSendUploadReadConfiguration();
            modelBuilder.ApplyConfiguration<SecureUploadsReadModel>(config);
            modelBuilder.ApplyConfiguration<UploadedFilesReadModel>(config);
        }
    }
}

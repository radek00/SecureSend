using Microsoft.EntityFrameworkCore;
using SecureSend.Domain.Entities;
using SecureSend.Infrastructure.EF.Config;

namespace SecureSend.Infrastructure.EF.Context
{
    internal sealed class SecureSendDbContext: DbContext
    {
        public DbSet<SecureSendUpload> SecureSendUploads { get; set; }

        public SecureSendDbContext(DbContextOptions<SecureSendDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("upload");
            var config = new SecureSendUploadConfiguration();
            modelBuilder.ApplyConfiguration<SecureSendUpload>(config);
        }
    }
}

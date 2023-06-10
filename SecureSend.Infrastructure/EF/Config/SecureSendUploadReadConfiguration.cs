using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureSend.Domain.ReadModels;

namespace SecureSend.Infrastructure.EF.Config
{
    internal sealed class SecureSendUploadReadConfiguration : IEntityTypeConfiguration<SecureUploadsReadModel>, IEntityTypeConfiguration<UploadedFilesReadModel>
    {
        public void Configure(EntityTypeBuilder<UploadedFilesReadModel> builder)
        {
            builder.ToTable("UploadedFiles");
            builder.HasKey(x => x.Id);
        }

        public void Configure(EntityTypeBuilder<SecureUploadsReadModel> builder)
        {
            builder.ToTable("SecureUploads");
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Files).WithOne(x => x.SecureUpload).HasForeignKey(x => x.SecureSendUploadId);
        }
    }
}

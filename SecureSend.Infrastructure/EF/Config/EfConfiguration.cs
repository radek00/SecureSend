using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Infrastructure.EF.Config
{
    internal static class SecureSendFields
    {
        internal static string uploadDate = "_uploadDate";
        internal static string expiryDate = "_expiryDate";
        internal static string isViewed = "_isViewedl";
        internal static string files = "_files";
    }
    internal sealed class EfConfiguration: IEntityTypeConfiguration<SecureSendUpload>
    {
        public void Configure(EntityTypeBuilder<SecureSendUpload> builder)
        {
            builder.HasKey(x => x.Id);

            var uploadDateConverter = new ValueConverter<SecureSendUploadDate, DateTime>(d => d.Value, d => new SecureSendUploadDate());
            var expiryDateConverter = new ValueConverter<SecureSendExpiryDate, DateTime?>(d => d.Value, d => new SecureSendExpiryDate(d));
            var isViewedConverter = new ValueConverter<SecureSendIsViewed, bool>(v => v.Value, v => new SecureSendIsViewed(v));

            builder.Property(p => p.Id).HasConversion(id => id.Value, id => new SecureSendUploadId(id));

            builder.Property(typeof(SecureSendUploadDate), SecureSendFields.uploadDate)
                .HasConversion(uploadDateConverter)
                .HasColumnName("UploadDate");


            builder.Property(typeof(SecureSendExpiryDate), SecureSendFields.expiryDate)
                .HasConversion(expiryDateConverter)
                .HasColumnName("ExpiryDate")
                .IsRequired(false);

            builder.Property(typeof(SecureSendIsViewed), SecureSendFields.isViewed)
                .HasConversion(isViewedConverter)
                .HasColumnName("IsViewed");

            builder.OwnsMany<SecureSendFile>(SecureSendFields.files, fileBuilder =>
            {
                fileBuilder.Property<int>("Id");
                fileBuilder.HasKey("Id");
                fileBuilder.Property(p => p.FileName);
                fileBuilder.Property(p => p.ContentType);

                fileBuilder.ToTable("UploadedFiles");

            });

            builder.ToTable("SecureUploads");

        }
    }
}

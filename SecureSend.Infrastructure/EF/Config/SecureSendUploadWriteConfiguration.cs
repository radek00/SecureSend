using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Infrastructure.EF.Config
{
    internal sealed class SecureSendUploadWriteConfiguration: IEntityTypeConfiguration<SecureSendUpload>
    {
        public void Configure(EntityTypeBuilder<SecureSendUpload> builder)
        {
            builder.HasKey(x => x.Id);

            var uploadDateConverter = new ValueConverter<SecureSendUploadDate, DateTime>(d => d.Value, d => new SecureSendUploadDate(d));
            var expiryDateConverter = new ValueConverter<SecureSendExpiryDate, DateTime?>(d => d.Value, d => new SecureSendExpiryDate(d));
            var passwordHashConverter =
                new ValueConverter<SecureSendPasswordHash, byte[]?>(v => v.Value, v => new SecureSendPasswordHash(v));

            builder.Property(p => p.Id).HasConversion(id => id.Value, id => new SecureSendUploadId(id));

            builder.Property(p => p.UploadDate)
                .HasConversion(uploadDateConverter)
                .HasColumnName("UploadDate");


            builder.Property(p => p.ExpiryDate)
                .HasConversion(expiryDateConverter!)
                .HasColumnName("ExpiryDate")
                .IsRequired(false);

            builder.Property(p => p.PasswordHash)
                .HasConversion(passwordHashConverter!)
                .HasColumnName("PasswordHash")
                .IsRequired(false);

            builder.OwnsMany<SecureSendFile>(p => p.Files, fileBuilder =>
            {
                fileBuilder.Property<int>("Id");
                fileBuilder.HasKey("Id");
                fileBuilder.Property(p => p.FileName);
                fileBuilder.Property(p => p.Metadata).IsRequired(true);

                fileBuilder.ToTable("UploadedFiles");

            });

            builder.ToTable("SecureUploads");

        }
    }
}

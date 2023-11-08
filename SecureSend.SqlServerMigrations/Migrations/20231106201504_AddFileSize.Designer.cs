﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecureSend.Infrastructure.EF.Context;

#nullable disable

namespace SecureSend.SqlServerMigration.Migrations
{
    [DbContext(typeof(SecureSendDbWriteContext))]
    [Migration("20231106201504_AddFileSize")]
    partial class AddFileSize
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("upload")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SecureSend.Domain.Entities.SecureSendUpload", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExpiryDate");

                    b.Property<bool>("IsViewed")
                        .HasColumnType("bit")
                        .HasColumnName("IsViewed");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("UploadDate");

                    b.HasKey("Id");

                    b.ToTable("SecureUploads", "upload");
                });

            modelBuilder.Entity("SecureSend.Domain.Entities.SecureSendUpload", b =>
                {
                    b.OwnsMany("SecureSend.Domain.ValueObjects.SecureSendFile", "Files", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("ContentType")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("FileName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<long>("FileSize")
                                .HasColumnType("bigint");

                            b1.Property<Guid>("SecureSendUploadId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("Id");

                            b1.HasIndex("SecureSendUploadId");

                            b1.ToTable("UploadedFiles", "upload");

                            b1.WithOwner()
                                .HasForeignKey("SecureSendUploadId");
                        });

                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}

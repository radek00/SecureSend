﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SecureSend.Infrastructure.EF.Context;

#nullable disable

namespace SecureSend.PostgresMigrations.Migrations
{
    [DbContext(typeof(SecureSendDbWriteContext))]
    partial class SecureSendDbWriteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("upload")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SecureSend.Domain.Entities.SecureSendUpload", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("ExpiryDate");

                    b.Property<bool>("IsViewed")
                        .HasColumnType("boolean")
                        .HasColumnName("IsViewed");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("bytea")
                        .HasColumnName("PasswordHash");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("timestamp with time zone")
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
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("ContentType")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("DisplayFileName")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<long>("FileSize")
                                .HasColumnType("bigint");

                            b1.Property<string>("RandomFileName")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<Guid>("SecureSendUploadId")
                                .HasColumnType("uuid");

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

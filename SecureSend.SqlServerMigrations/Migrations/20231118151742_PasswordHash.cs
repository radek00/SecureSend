﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureSend.SqlServerMigration.Migrations
{
    /// <inheritdoc />
    public partial class PasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                schema: "upload",
                table: "SecureUploads",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "upload",
                table: "SecureUploads");
        }
    }
}

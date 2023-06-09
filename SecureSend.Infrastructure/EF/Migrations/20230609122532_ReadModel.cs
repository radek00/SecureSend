using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureSend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "upload");

            migrationBuilder.CreateTable(
                name: "SecureUploads",
                schema: "upload",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsViewed = table.Column<bool>(type: "bit", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecureUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                schema: "upload",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecureSendUploadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_SecureUploads_SecureSendUploadId",
                        column: x => x.SecureSendUploadId,
                        principalSchema: "upload",
                        principalTable: "SecureUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_SecureSendUploadId",
                schema: "upload",
                table: "UploadedFiles",
                column: "SecureSendUploadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadedFiles",
                schema: "upload");

            migrationBuilder.DropTable(
                name: "SecureUploads",
                schema: "upload");
        }
    }
}

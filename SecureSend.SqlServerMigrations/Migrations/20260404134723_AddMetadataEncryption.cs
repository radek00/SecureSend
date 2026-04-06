using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureSend.SqlServerMigration.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadataEncryption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                schema: "upload",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "upload",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "IsViewed",
                schema: "upload",
                table: "SecureUploads");

            migrationBuilder.RenameColumn(
                name: "RandomFileName",
                schema: "upload",
                table: "UploadedFiles",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "DisplayFileName",
                schema: "upload",
                table: "UploadedFiles",
                newName: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Metadata",
                schema: "upload",
                table: "UploadedFiles",
                newName: "RandomFileName");

            migrationBuilder.RenameColumn(
                name: "FileName",
                schema: "upload",
                table: "UploadedFiles",
                newName: "DisplayFileName");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                schema: "upload",
                table: "UploadedFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                schema: "upload",
                table: "UploadedFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsViewed",
                schema: "upload",
                table: "SecureUploads",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureSend.SqlServerMigration.Migrations
{
    /// <inheritdoc />
    public partial class RandomFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                schema: "upload",
                table: "UploadedFiles",
                newName: "RandomFileName");

            migrationBuilder.AddColumn<string>(
                name: "DisplayFileName",
                schema: "upload",
                table: "UploadedFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayFileName",
                schema: "upload",
                table: "UploadedFiles");

            migrationBuilder.RenameColumn(
                name: "RandomFileName",
                schema: "upload",
                table: "UploadedFiles",
                newName: "FileName");
        }
    }
}

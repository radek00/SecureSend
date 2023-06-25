using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureSend.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class PasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "upload",
                table: "SecureUploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
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

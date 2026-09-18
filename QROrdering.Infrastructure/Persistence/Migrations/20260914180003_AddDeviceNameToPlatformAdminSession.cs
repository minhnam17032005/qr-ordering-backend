using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QROrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceNameToPlatformAdminSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "PlatformAdminSessions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "PlatformAdminSessions");
        }
    }
}

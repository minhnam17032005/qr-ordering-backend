using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QROrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveJoinTokenHashOnCustomerSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerSessions_JoinTokenHash",
                table: "CustomerSessions");

            migrationBuilder.DropColumn(
                name: "JoinTokenHash",
                table: "CustomerSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JoinTokenHash",
                table: "CustomerSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSessions_JoinTokenHash",
                table: "CustomerSessions",
                column: "JoinTokenHash",
                unique: true);
        }
    }
}

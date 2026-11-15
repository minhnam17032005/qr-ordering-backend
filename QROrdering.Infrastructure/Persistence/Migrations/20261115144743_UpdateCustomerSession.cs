using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QROrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerSessions_SessionToken",
                table: "CustomerSessions");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CustomerSessions");

            migrationBuilder.DropColumn(
                name: "SessionToken",
                table: "CustomerSessions");

            migrationBuilder.AddColumn<string>(
                name: "JoinTokenHash",
                table: "CustomerSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SessionTokenHash",
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

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSessions_SessionTokenHash",
                table: "CustomerSessions",
                column: "SessionTokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerSessions_JoinTokenHash",
                table: "CustomerSessions");

            migrationBuilder.DropIndex(
                name: "IX_CustomerSessions_SessionTokenHash",
                table: "CustomerSessions");

            migrationBuilder.DropColumn(
                name: "JoinTokenHash",
                table: "CustomerSessions");

            migrationBuilder.DropColumn(
                name: "SessionTokenHash",
                table: "CustomerSessions");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "CustomerSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionToken",
                table: "CustomerSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSessions_SessionToken",
                table: "CustomerSessions",
                column: "SessionToken",
                unique: true);
        }
    }
}

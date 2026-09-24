using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QROrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceRegistrationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RestaurantAddress",
                table: "ServiceRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantDescription",
                table: "ServiceRegistrations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantEmail",
                table: "ServiceRegistrations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantLogoUrl",
                table: "ServiceRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantPhoneNumber",
                table: "ServiceRegistrations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ServiceRegistrations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRegistrations_UserId",
                table: "ServiceRegistrations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRegistrations_Users_UserId",
                table: "ServiceRegistrations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRegistrations_Users_UserId",
                table: "ServiceRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRegistrations_UserId",
                table: "ServiceRegistrations");

            migrationBuilder.DropColumn(
                name: "RestaurantDescription",
                table: "ServiceRegistrations");

            migrationBuilder.DropColumn(
                name: "RestaurantEmail",
                table: "ServiceRegistrations");

            migrationBuilder.DropColumn(
                name: "RestaurantLogoUrl",
                table: "ServiceRegistrations");

            migrationBuilder.DropColumn(
                name: "RestaurantPhoneNumber",
                table: "ServiceRegistrations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ServiceRegistrations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Roles");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantAddress",
                table: "ServiceRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}

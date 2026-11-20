using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QROrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Payments",
                newName: "RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                newName: "IX_Payments_RestaurantId");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerSessionId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RestaurantId_CustomerSessionId",
                table: "Payments",
                columns: new[] { "RestaurantId", "CustomerSessionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CustomerSessions_RestaurantId_CustomerSessionId",
                table: "Payments",
                columns: new[] { "RestaurantId", "CustomerSessionId" },
                principalTable: "CustomerSessions",
                principalColumns: new[] { "RestaurantId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Restaurants_RestaurantId",
                table: "Payments",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CustomerSessions_RestaurantId_CustomerSessionId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Restaurants_RestaurantId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RestaurantId_CustomerSessionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomerSessionId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "Payments",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_RestaurantId",
                table: "Payments",
                newName: "IX_Payments_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}

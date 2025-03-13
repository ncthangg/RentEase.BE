using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Transaction",
                newName: "PaymentStatusId");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Orders",
                newName: "PaymentStatusId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelleddAt",
                table: "Transaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelleddAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelleddAt",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CancelleddAt",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PaymentStatusId",
                table: "Transaction",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "PaymentStatusId",
                table: "Orders",
                newName: "StatusId");
        }
    }
}

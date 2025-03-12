using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountToken_Account",
                table: "AccountToken");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountVerification_Account",
                table: "AccountVerification");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountToken_Account",
                table: "AccountToken",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountVerification_Account",
                table: "AccountVerification",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountToken_Account",
                table: "AccountToken");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountVerification_Account",
                table: "AccountVerification");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountToken_Account",
                table: "AccountToken",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountVerification_Account",
                table: "AccountVerification",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId");
        }
    }
}

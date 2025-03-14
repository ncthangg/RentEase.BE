using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AptImage_AptId",
                table: "AptImage");

            migrationBuilder.CreateIndex(
                name: "IX_AptImage_AptId",
                table: "AptImage",
                column: "AptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AptImage_AptId",
                table: "AptImage");

            migrationBuilder.CreateIndex(
                name: "IX_AptImage_AptId",
                table: "AptImage",
                column: "AptId",
                unique: true,
                filter: "[AptId] IS NOT NULL");
        }
    }
}

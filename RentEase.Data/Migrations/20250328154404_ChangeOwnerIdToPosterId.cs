using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOwnerIdToPosterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Post",
                newName: "PosterId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_AccountId",
                table: "Post",
                newName: "IX_Post_PosterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PosterId",
                table: "Post",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_PosterId",
                table: "Post",
                newName: "IX_Post_AccountId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversationAndMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId2",
                table: "Conversations",
                newName: "AccountId2");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Conversations",
                newName: "AccountId1");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Conversations",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Conversations",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "AccountId2",
                table: "Conversations",
                newName: "UserId2");

            migrationBuilder.RenameColumn(
                name: "AccountId1",
                table: "Conversations",
                newName: "UserId1");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOwnerIdToPosterId_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                 name: "OwnerId",
                 table: "Apt",
                 newName: "PosterId"
             );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                 name: "PosterId",
                 table: "Apt",
                 newName: "OwnerId"
             );
        }
    }
}

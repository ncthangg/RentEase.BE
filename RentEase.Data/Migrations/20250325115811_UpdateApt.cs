using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "Apt",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "Apt");
        }
    }
}

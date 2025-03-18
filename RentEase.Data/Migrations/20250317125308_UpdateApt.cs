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
            migrationBuilder.DropForeignKey(
                name: "FK_AccountLikedApt_Account_AccountId",
                table: "AccountLikedApt");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountLikedApt_Apt_AptId",
                table: "AccountLikedApt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountLikedApt",
                table: "AccountLikedApt");

            migrationBuilder.RenameTable(
                name: "AccountLikedApt",
                newName: "AccountLikedApts");

            migrationBuilder.RenameIndex(
                name: "IX_AccountLikedApt_AptId",
                table: "AccountLikedApts",
                newName: "IX_AccountLikedApts_AptId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountLikedApt_AccountId",
                table: "AccountLikedApts",
                newName: "IX_AccountLikedApts_AccountId");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Apt",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Apt",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerPhone",
                table: "Apt",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountLikedApts",
                table: "AccountLikedApts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLikedApts_Account_AccountId",
                table: "AccountLikedApts",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLikedApts_Apt_AptId",
                table: "AccountLikedApts",
                column: "AptId",
                principalTable: "Apt",
                principalColumn: "AptId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountLikedApts_Account_AccountId",
                table: "AccountLikedApts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountLikedApts_Apt_AptId",
                table: "AccountLikedApts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountLikedApts",
                table: "AccountLikedApts");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Apt");

            migrationBuilder.DropColumn(
                name: "OwnerPhone",
                table: "Apt");

            migrationBuilder.RenameTable(
                name: "AccountLikedApts",
                newName: "AccountLikedApt");

            migrationBuilder.RenameIndex(
                name: "IX_AccountLikedApts_AptId",
                table: "AccountLikedApt",
                newName: "IX_AccountLikedApt_AptId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountLikedApts_AccountId",
                table: "AccountLikedApt",
                newName: "IX_AccountLikedApt_AccountId");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Apt",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountLikedApt",
                table: "AccountLikedApt",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLikedApt_Account_AccountId",
                table: "AccountLikedApt",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLikedApt_Apt_AptId",
                table: "AccountLikedApt",
                column: "AptId",
                principalTable: "Apt",
                principalColumn: "AptId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

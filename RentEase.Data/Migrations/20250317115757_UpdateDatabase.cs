using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Account",
                newName: "GenderId"
            );
            migrationBuilder.AddColumn<int>(
                name: "PostCategoryId",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Orders",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GenderId",
                table: "Account",
                type: "int",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountLikedApt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    AptId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLikedApt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountLikedApt_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountLikedApt_Apt_AptId",
                        column: x => x.AptId,
                        principalTable: "Apt",
                        principalColumn: "AptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostCategoryId",
                table: "Post",
                column: "PostCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PostId",
                table: "Orders",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLikedApt_AccountId",
                table: "AccountLikedApt",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLikedApt_AptId",
                table: "AccountLikedApt",
                column: "AptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Post_PostId",
                table: "Orders",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostCategory_PostCategoryId",
                table: "Post",
                column: "PostCategoryId",
                principalTable: "PostCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Post_PostId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostCategory_PostCategoryId",
                table: "Post");

            migrationBuilder.DropTable(
                name: "AccountLikedApt");

            migrationBuilder.DropTable(
                name: "PostCategory");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostCategoryId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PostId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PostCategoryId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "GenderId",
                table: "Account",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}

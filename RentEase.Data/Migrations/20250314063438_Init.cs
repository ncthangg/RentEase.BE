using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentEase.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AptCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AptCateg__3214EC07B602F106", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AptStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AptStatu__3214EC0701430943", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__3214EC074195CB0F", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transact__3214EC07450B5CD3", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Utility__3214EC07BD26B4C7", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Apt",
                columns: table => new
                {
                    AptId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Area = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AddressLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    WardId = table.Column<int>(type: "int", nullable: false),
                    RentPrice = table.Column<long>(type: "bigint", nullable: false),
                    PilePrice = table.Column<long>(type: "bigint", nullable: true),
                    AptCategoryId = table.Column<int>(type: "int", nullable: false),
                    AptStatusId = table.Column<int>(type: "int", nullable: false),
                    NumberOfRoom = table.Column<int>(type: "int", nullable: false),
                    NumberOfSlot = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Apt__8D24E77238D6C477", x => x.AptId);
                    table.ForeignKey(
                        name: "FK_Apt_AptCategory",
                        column: x => x.AptCategoryId,
                        principalTable: "AptCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apt_AptStatus",
                        column: x => x.AptStatusId,
                        principalTable: "AptStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__349DA5A62F5DEB04", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Account_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AptImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AptId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AptImage__8D24E772761F5E0C", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AptImage_Apt",
                        column: x => x.AptId,
                        principalTable: "Apt",
                        principalColumn: "AptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AptUtility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AptId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UtilityId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AptUtili__3214EC07540A0EA3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AptUtility_Apt",
                        column: x => x.AptId,
                        principalTable: "Apt",
                        principalColumn: "AptId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AptUtility_Utility",
                        column: x => x.UtilityId,
                        principalTable: "Utility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AccountT__3214EC074C2E87F3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountToken_Account",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountVerification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AccountV__3214EC07812979E0", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountVerification_Account",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncurredCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CancelleddAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__C3905BCF74207269", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_Sender",
                        column: x => x.SenderId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_Order_TransactionType",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AptId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TotalSlot = table.Column<int>(type: "int", nullable: false),
                    CurrentSlot = table.Column<int>(type: "int", nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoveInDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MoveOutDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post__AA1260182BDDB060", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_CPost_Account",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_Post_Apt",
                        column: x => x.AptId,
                        principalTable: "Apt",
                        principalColumn: "AptId");
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AptId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review__3214EC071860634A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Account",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_Review_Apt",
                        column: x => x.AptId,
                        principalTable: "Apt",
                        principalColumn: "AptId");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PaymentAttempt = table.Column<int>(type: "int", nullable: false),
                    PaymentCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CancelleddAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transact__3214EC07438D024A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_Orders",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_TransactionType",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PostRequire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PostRequ__3214EC0721C7FA81", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostRequire_Account",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_PostRequire_Post",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "PostId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Email",
                table: "Account",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Account_PhoneNumber",
                table: "Account",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleId",
                table: "Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__Account__85FB4E38627969EB",
                table: "Account",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "UQ__Account__A9D1053441F530BF",
                table: "Account",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AccountToken_AccountId",
                table: "AccountToken",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountVerification_AccountId",
                table: "AccountVerification",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Apt_AptCategory",
                table: "Apt",
                column: "AptCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Apt_AptStatus",
                table: "Apt",
                column: "AptStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Apt_OwnerId",
                table: "Apt",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "UQ__AptCateg__8517B2E008B30AFE",
                table: "AptCategory",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AptImage_AptId",
                table: "AptImage",
                column: "AptId",
                unique: true,
                filter: "[AptId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__AptStatu__05E7698A86EA1D6C",
                table: "AptStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AptUtility_AptId",
                table: "AptUtility",
                column: "AptId");

            migrationBuilder.CreateIndex(
                name: "IX_AptUtility_UtilityId",
                table: "AptUtility",
                column: "UtilityId");

            migrationBuilder.CreateIndex(
                name: "UQ_AptUtility",
                table: "AptUtility",
                columns: new[] { "AptId", "UtilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SenderId",
                table: "Orders",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TransactionTypeId",
                table: "Orders",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_AccountId",
                table: "Post",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_AptId",
                table: "Post",
                column: "AptId");

            migrationBuilder.CreateIndex(
                name: "IX_PostRequire_AccountId",
                table: "PostRequire",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PostRequire_PostId",
                table: "PostRequire",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AccountId",
                table: "Review",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AptId",
                table: "Review",
                column: "AptId");

            migrationBuilder.CreateIndex(
                name: "UQ__Role__8A2B616060E7C55B",
                table: "Role",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_OrderId",
                table: "Transaction",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TransactionTypeId",
                table: "Transaction",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "UQ__Transact__D4E7DFA8FCA6D65C",
                table: "TransactionType",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Utility__E8B225D6E81AEADB",
                table: "Utility",
                column: "UtilityName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountToken");

            migrationBuilder.DropTable(
                name: "AccountVerification");

            migrationBuilder.DropTable(
                name: "AptImage");

            migrationBuilder.DropTable(
                name: "AptUtility");

            migrationBuilder.DropTable(
                name: "PostRequire");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Utility");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Apt");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "TransactionType");

            migrationBuilder.DropTable(
                name: "AptCategory");

            migrationBuilder.DropTable(
                name: "AptStatus");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}

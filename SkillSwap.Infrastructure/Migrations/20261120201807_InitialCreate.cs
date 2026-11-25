using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillSwap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SkillCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillListings_SkillCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SkillCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillListings_UserProfiles_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    AvailableCredits = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReservedCredits = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillListingId = table.Column<int>(type: "int", nullable: false),
                    LearnerId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreditCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_SkillListings_SkillListingId",
                        column: x => x.SkillListingId,
                        principalTable: "SkillListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_UserProfiles_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: true),
                    FromWalletId = table.Column<int>(type: "int", nullable: true),
                    ToWalletId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditTransactions_Wallets_FromWalletId",
                        column: x => x.FromWalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_Wallets_ToWalletId",
                        column: x => x.ToWalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_UserProfiles_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SkillCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Programming skills", "Programming" },
                    { 2, "Music skills", "Music" },
                    { 3, "Design skills", "Design" },
                    { 4, "Languages skills", "Languages" },
                    { 5, "Academic skills", "Academic" },
                    { 6, "Wellness skills", "Wellness" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "Bio", "CreatedAt", "DisplayName", "Email", "IsActive" },
                values: new object[,]
                {
                    { 1, "A SkillSwap demo student.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Student 1", "student1@skillswap.demo", true },
                    { 2, "A SkillSwap demo student.", new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Student 2", "student2@skillswap.demo", true },
                    { 3, "A SkillSwap demo student.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Student 3", "student3@skillswap.demo", true },
                    { 4, "A SkillSwap demo student.", new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Student 4", "student4@skillswap.demo", true },
                    { 5, "A SkillSwap demo student.", new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Student 5", "student5@skillswap.demo", true },
                    { 6, "A SkillSwap demo student.", new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Student 6", "student6@skillswap.demo", true }
                });

            migrationBuilder.InsertData(
                table: "SkillListings",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsActive", "TeacherId", "Title" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 2, "Python fundamentals" },
                    { 2, 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 3, "Guitar basics" },
                    { 3, 4, new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 4, "Photoshop studio" },
                    { 4, 5, new DateTime(2025, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 5, "French conversation" },
                    { 5, 6, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 6, "Statistics help" },
                    { 6, 1, new DateTime(2025, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 1, "Public speaking" },
                    { 7, 2, new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 2, "Python fundamentals" },
                    { 8, 3, new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 3, "Guitar basics" },
                    { 9, 4, new DateTime(2025, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 4, "Photoshop studio" },
                    { 10, 5, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 5, "French conversation" },
                    { 11, 6, new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 6, "Statistics help" },
                    { 12, 1, new DateTime(2025, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), "A friendly one-hour peer learning session.", true, 1, "Public speaking" }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "AvailableCredits", "ReservedCredits", "UserProfileId" },
                values: new object[,]
                {
                    { 1, 3m, 0m, 1 },
                    { 2, 3m, 0m, 2 },
                    { 3, 3m, 0m, 3 },
                    { 4, 3m, 0m, 4 },
                    { 5, 3m, 0m, 5 },
                    { 6, 3m, 0m, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LearnerId",
                table: "Bookings",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SkillListingId",
                table: "Bookings",
                column: "SkillListingId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_BookingId",
                table: "CreditTransactions",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_FromWalletId",
                table: "CreditTransactions",
                column: "FromWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_ToWalletId",
                table: "CreditTransactions",
                column: "ToWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId_ReviewerId",
                table: "Reviews",
                columns: new[] { "BookingId", "ReviewerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillListings_CategoryId",
                table: "SkillListings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillListings_TeacherId",
                table: "SkillListings",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Email",
                table: "UserProfiles",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserProfileId",
                table: "Wallets",
                column: "UserProfileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditTransactions");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "SkillListings");

            migrationBuilder.DropTable(
                name: "SkillCategories");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}

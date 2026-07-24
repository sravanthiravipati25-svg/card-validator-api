using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Validation.CardService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardValidationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CardNumberHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    IssuerNetwork = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValidatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BatchId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardValidationRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardValidationRecords_BatchId",
                table: "CardValidationRecords",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CardValidationRecords_ValidatedAtUtc",
                table: "CardValidationRecords",
                column: "ValidatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardValidationRecords");
        }
    }
}

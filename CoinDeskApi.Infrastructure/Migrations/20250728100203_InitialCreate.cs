using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoinDeskApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ChineseName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "ChineseName", "CreatedAt", "EnglishName", "Symbol", "UpdatedAt" },
                values: new object[,]
                {
                    { "EUR", "歐元", new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9053), "Euro", "€", new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9054) },
                    { "GBP", "英鎊", new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9056), "British Pound Sterling", "£", new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9056) },
                    { "USD", "美元", new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(8161), "US Dollar", "$", new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(8476) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Id",
                table: "Currencies",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}

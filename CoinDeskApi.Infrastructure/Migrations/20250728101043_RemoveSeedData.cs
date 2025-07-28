using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoinDeskApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "EUR");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "GBP");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "USD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "ChineseName", "CreatedAt", "EnglishName", "Symbol", "UpdatedAt" },
                values: new object[,]
                {
                    { "EUR", "歐元", new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", "€", new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "GBP", "英鎊", new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "British Pound Sterling", "£", new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "USD", "美元", new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "US Dollar", "$", new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}

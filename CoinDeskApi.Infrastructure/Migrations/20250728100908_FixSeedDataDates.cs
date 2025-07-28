using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoinDeskApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedDataDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "EUR",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "GBP",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "USD",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "EUR",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9053), new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9054) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "GBP",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9056), new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(9056) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: "USD",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(8161), new DateTime(2025, 7, 28, 10, 2, 3, 204, DateTimeKind.Utc).AddTicks(8476) });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "NodeID", "Name" },
                values: new object[,]
                {
                    { new Guid("72c4473f-a855-41b5-a777-66c6b0497325"), "ORDER.API" },
                    { new Guid("8b9471f5-b5fe-4116-a4b1-cc496e2c1dd8"), "PAYMENT.API" },
                    { new Guid("b6fa082f-ad61-4456-91e7-da6e5e74732e"), "STOCK.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "NodeID",
                keyValue: new Guid("72c4473f-a855-41b5-a777-66c6b0497325"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "NodeID",
                keyValue: new Guid("8b9471f5-b5fe-4116-a4b1-cc496e2c1dd8"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "NodeID",
                keyValue: new Guid("b6fa082f-ad61-4456-91e7-da6e5e74732e"));
        }
    }
}

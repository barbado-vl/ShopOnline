using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopOnline.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8061452f-3867-4378-9220-ba9194881da8",
                column: "ConcurrencyStamp",
                value: "2c4c84a9-5473-486a-aa72-9505dba40b1d");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3bb008b3-c127-47b6-8ad6-59badbfd8d10",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0f785bea-fca5-474f-afac-936252a2d2b7", "AQAAAAEAACcQAAAAEHNlT0Ec4ZQIKNIWrNWUp2MJq2Faf4dojtuqQlaqq5HPiPeNTcyMkUKZLMPtRuNt8g==" });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CustomerId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8061452f-3867-4378-9220-ba9194881da8",
                column: "ConcurrencyStamp",
                value: "3cd45245-2ee6-407b-a1c9-700ae4eb87e3");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3bb008b3-c127-47b6-8ad6-59badbfd8d10",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "24760bb8-0150-48ee-86f8-133a4e947a9c", "AQAAAAEAACcQAAAAEK41GRP3QjJ7xj5iKr3ndsyWsTR8fjk4IWfGY1lutj0+GvBalN9UwCS7GXhj+O1yTQ==" });
        }
    }
}

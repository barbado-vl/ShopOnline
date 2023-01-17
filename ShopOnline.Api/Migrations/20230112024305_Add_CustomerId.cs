using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopOnline.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Carts",
                newName: "CustomerId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Carts",
                newName: "UserId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8061452f-3867-4378-9220-ba9194881da8",
                column: "ConcurrencyStamp",
                value: "771adc56-3f03-4a23-98c5-9b878e64d1bf");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3bb008b3-c127-47b6-8ad6-59badbfd8d10",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fd2761dc-c568-4fea-8431-800cf16848ec", "AQAAAAEAACcQAAAAEKVHhMlP499HK73owdS3JgjjeFdRKPmQlSdcG2CtWDaLsyrPtfhtAWXwqMzxMlKPIw==" });
        }
    }
}

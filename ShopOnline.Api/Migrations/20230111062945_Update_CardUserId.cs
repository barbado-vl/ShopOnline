using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopOnline.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCardUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Carts",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8061452f-3867-4378-9220-ba9194881da8",
                column: "ConcurrencyStamp",
                value: "6221f0b0-b2e1-4f27-974e-1abf2d151444");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3bb008b3-c127-47b6-8ad6-59badbfd8d10",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5dd459fd-0271-467d-b0a4-0162c89a97b2", "AQAAAAEAACcQAAAAEDXdJ9dfCpCXLOtiyNBzU/C4qjXvYcRSKpgWNxZiZNc06BfyNHIdjANP9kD/ylxjhQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8061452f-3867-4378-9220-ba9194881da8",
                column: "ConcurrencyStamp",
                value: "f4439fde-7c7a-4ae5-a1ae-32646e7e77b0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3bb008b3-c127-47b6-8ad6-59badbfd8d10",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "be881eec-44f0-47be-ae8d-49be43b9e4ae", "AQAAAAEAACcQAAAAED/2zkBKY9JR6S1W/ePBjo1eTno9+SSQq5R066FwCgxzjhvpAnTMcrCRWLq6QKJUAg==" });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "UserId" },
                values: new object[] { 1, new Guid("3bb008b3-c127-47b6-8ad6-59badbfd8d10") });
        }
    }
}

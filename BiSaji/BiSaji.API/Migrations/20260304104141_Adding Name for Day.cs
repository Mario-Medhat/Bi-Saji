using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiSaji.API.Migrations
{
    /// <inheritdoc />
    public partial class AddingNameforDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "abcdef12-3456-7890-abcd-ef1234567890");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Days");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Days",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Days");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Days",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                schema: "auth",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "abcdef12-3456-7890-abcd-ef1234567890", "abcdef12-3456-7890-abcd-ef1234567890", "BatchLeader", "BATCHLEADER" });
        }
    }
}

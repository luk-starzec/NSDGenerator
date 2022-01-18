using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSDGenerator.Server.Migrations
{
    public partial class ColumnWidths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "nsd",
                table: "Users",
                keyColumn: "Name",
                keyValue: "user@starzec.net");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "nsd",
                table: "Users",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "nsd",
                table: "Diagram",
                type: "nvarchar(400)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)");

            migrationBuilder.AddColumn<string>(
                name: "ColumnWidths",
                schema: "nsd",
                table: "Diagram",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "nsd",
                table: "RegistrationCodes",
                keyColumn: "Code",
                keyValue: "test01",
                column: "IsActive",
                value: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnWidths",
                schema: "nsd",
                table: "Diagram");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "nsd",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "nsd",
                table: "Diagram",
                type: "nvarchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)");

            migrationBuilder.UpdateData(
                schema: "nsd",
                table: "RegistrationCodes",
                keyColumn: "Code",
                keyValue: "test01",
                column: "IsActive",
                value: true);

            migrationBuilder.InsertData(
                schema: "nsd",
                table: "Users",
                columns: new[] { "Name", "Created", "IsEnabled", "Password" },
                values: new object[] { "user@starzec.net", new DateTime(2022, 1, 11, 18, 56, 16, 650, DateTimeKind.Local).AddTicks(8281), true, "123" });
        }
    }
}

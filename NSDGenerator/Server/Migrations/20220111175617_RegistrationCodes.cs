using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSDGenerator.Server.Migrations
{
    public partial class RegistrationCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrationCodes",
                schema: "nsd",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationCodes", x => x.Code);
                });

            migrationBuilder.InsertData(
                schema: "nsd",
                table: "RegistrationCodes",
                columns: new[] { "Code", "IsActive", "ValidTo" },
                values: new object[] { "test01", true, null });

            migrationBuilder.UpdateData(
                schema: "nsd",
                table: "Users",
                keyColumn: "Name",
                keyValue: "user@starzec.net",
                column: "Created",
                value: new DateTime(2022, 1, 11, 18, 56, 16, 650, DateTimeKind.Local).AddTicks(8281));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationCodes",
                schema: "nsd");

            migrationBuilder.UpdateData(
                schema: "nsd",
                table: "Users",
                keyColumn: "Name",
                keyValue: "user@starzec.net",
                column: "Created",
                value: new DateTime(2022, 1, 8, 18, 9, 45, 64, DateTimeKind.Local).AddTicks(6522));
        }
    }
}

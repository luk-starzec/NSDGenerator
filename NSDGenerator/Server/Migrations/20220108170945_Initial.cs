using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSDGenerator.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "nsd");

            migrationBuilder.CreateTable(
                name: "Blocks",
                schema: "nsd",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiagramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "nsd",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Diagram",
                schema: "nsd",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    RootBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagram", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagram_Blocks_RootBlockId",
                        column: x => x.RootBlockId,
                        principalSchema: "nsd",
                        principalTable: "Blocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Diagram_Users_UserName",
                        column: x => x.UserName,
                        principalSchema: "nsd",
                        principalTable: "Users",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "nsd",
                table: "Users",
                columns: new[] { "Name", "Created", "IsEnabled", "Password" },
                values: new object[] { "user@starzec.net", new DateTime(2022, 1, 8, 18, 9, 45, 64, DateTimeKind.Local).AddTicks(6522), true, "123" });

            migrationBuilder.CreateIndex(
                name: "IX_Diagram_RootBlockId",
                schema: "nsd",
                table: "Diagram",
                column: "RootBlockId",
                unique: true,
                filter: "[RootBlockId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Diagram_UserName",
                schema: "nsd",
                table: "Diagram",
                column: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagram",
                schema: "nsd");

            migrationBuilder.DropTable(
                name: "Blocks",
                schema: "nsd");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "nsd");
        }
    }
}

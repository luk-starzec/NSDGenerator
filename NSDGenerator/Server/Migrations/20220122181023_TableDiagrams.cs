using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSDGenerator.Server.Migrations
{
    public partial class TableDiagrams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagram_Blocks_RootBlockId",
                schema: "nsd",
                table: "Diagram");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagram_Users_UserName",
                schema: "nsd",
                table: "Diagram");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagram",
                schema: "nsd",
                table: "Diagram");

            migrationBuilder.RenameTable(
                name: "Diagram",
                schema: "nsd",
                newName: "Diagrams",
                newSchema: "nsd");

            migrationBuilder.RenameColumn(
                name: "ColumnWidths",
                schema: "nsd",
                table: "Diagrams",
                newName: "ColumnsWidth");

            migrationBuilder.RenameIndex(
                name: "IX_Diagram_UserName",
                schema: "nsd",
                table: "Diagrams",
                newName: "IX_Diagrams_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Diagram_RootBlockId",
                schema: "nsd",
                table: "Diagrams",
                newName: "IX_Diagrams_RootBlockId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagrams",
                schema: "nsd",
                table: "Diagrams",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagrams_Blocks_RootBlockId",
                schema: "nsd",
                table: "Diagrams",
                column: "RootBlockId",
                principalSchema: "nsd",
                principalTable: "Blocks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagrams_Users_UserName",
                schema: "nsd",
                table: "Diagrams",
                column: "UserName",
                principalSchema: "nsd",
                principalTable: "Users",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagrams_Blocks_RootBlockId",
                schema: "nsd",
                table: "Diagrams");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagrams_Users_UserName",
                schema: "nsd",
                table: "Diagrams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagrams",
                schema: "nsd",
                table: "Diagrams");

            migrationBuilder.RenameTable(
                name: "Diagrams",
                schema: "nsd",
                newName: "Diagram",
                newSchema: "nsd");

            migrationBuilder.RenameColumn(
                name: "ColumnsWidth",
                schema: "nsd",
                table: "Diagram",
                newName: "ColumnWidths");

            migrationBuilder.RenameIndex(
                name: "IX_Diagrams_UserName",
                schema: "nsd",
                table: "Diagram",
                newName: "IX_Diagram_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Diagrams_RootBlockId",
                schema: "nsd",
                table: "Diagram",
                newName: "IX_Diagram_RootBlockId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagram",
                schema: "nsd",
                table: "Diagram",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagram_Blocks_RootBlockId",
                schema: "nsd",
                table: "Diagram",
                column: "RootBlockId",
                principalSchema: "nsd",
                principalTable: "Blocks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagram_Users_UserName",
                schema: "nsd",
                table: "Diagram",
                column: "UserName",
                principalSchema: "nsd",
                principalTable: "Users",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

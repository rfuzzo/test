using Microsoft.EntityFrameworkCore.Migrations;

namespace RedDatabase.Migrations
{
    public partial class Test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    RedFileId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.RedFileId);
                });

            migrationBuilder.CreateTable(
                name: "RedFileRedFile",
                columns: table => new
                {
                    UsedByRedFileId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UsesRedFileId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedFileRedFile", x => new { x.UsedByRedFileId, x.UsesRedFileId });
                    table.ForeignKey(
                        name: "FK_RedFileRedFile_Files_UsedByRedFileId",
                        column: x => x.UsedByRedFileId,
                        principalTable: "Files",
                        principalColumn: "RedFileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RedFileRedFile_Files_UsesRedFileId",
                        column: x => x.UsesRedFileId,
                        principalTable: "Files",
                        principalColumn: "RedFileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RedFileRedFile_UsesRedFileId",
                table: "RedFileRedFile",
                column: "UsesRedFileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RedFileRedFile");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}

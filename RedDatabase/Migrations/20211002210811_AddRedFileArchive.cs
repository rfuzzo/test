using Microsoft.EntityFrameworkCore.Migrations;

namespace RedDatabase.Migrations
{
    public partial class AddRedFileArchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Archive",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archive",
                table: "Files");
        }
    }
}

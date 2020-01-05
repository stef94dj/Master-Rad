using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class removeSqlScriptcolumnfromAnalysisPapertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SqlScript",
                table: "AnalysisPaper");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SqlScript",
                table: "AnalysisPaper",
                nullable: true);
        }
    }
}

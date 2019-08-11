using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class addedNametoTestEntitites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SynthesisTest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AnalysisTest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SynthesisTest");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AnalysisTest");
        }
    }
}

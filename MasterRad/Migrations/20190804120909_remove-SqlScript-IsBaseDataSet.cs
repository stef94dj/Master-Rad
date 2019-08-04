using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class removeSqlScriptIsBaseDataSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBaseDataSet",
                table: "DbTemplate");

            migrationBuilder.DropColumn(
                name: "SqlScript",
                table: "DbTemplate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBaseDataSet",
                table: "DbTemplate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SqlScript",
                table: "DbTemplate",
                nullable: true);
        }
    }
}

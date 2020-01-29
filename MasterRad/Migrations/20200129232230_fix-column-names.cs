using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class fixcolumnnames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InputNameOnServer",
                table: "SynthesisTestStudent",
                newName: "NameOnServer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameOnServer",
                table: "SynthesisTestStudent",
                newName: "InputNameOnServer");
        }
    }
}

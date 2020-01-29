using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class renameNameOnServercolumninAnalysisTestStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameOnServer",
                table: "SynthesisTestStudent",
                newName: "InputNameOnServer");

            migrationBuilder.RenameColumn(
                name: "NameOnServer",
                table: "AnalysisTestStudent",
                newName: "InputNameOnServer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InputNameOnServer",
                table: "SynthesisTestStudent",
                newName: "NameOnServer");

            migrationBuilder.RenameColumn(
                name: "InputNameOnServer",
                table: "AnalysisTestStudent",
                newName: "NameOnServer");
        }
    }
}

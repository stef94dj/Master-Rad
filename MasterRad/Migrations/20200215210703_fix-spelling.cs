using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class fixspelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrepareOtputsProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.RenameColumn(
                name: "PrepareOtputsFailReason",
                table: "AnalysisTestStudent",
                newName: "PrepareOutputsFailReason");

            migrationBuilder.AddColumn<int>(
                name: "PrepareOutputsProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrepareOutputsProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.RenameColumn(
                name: "PrepareOutputsFailReason",
                table: "AnalysisTestStudent",
                newName: "PrepareOtputsFailReason");

            migrationBuilder.AddColumn<int>(
                name: "PrepareOtputsProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);
        }
    }
}

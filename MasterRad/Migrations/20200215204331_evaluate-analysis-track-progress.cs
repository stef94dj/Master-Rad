using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class evaluateanalysistrackprogress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailingInputFailReason",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrepareOtputsFailReason",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrepareOtputsProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StudentOutputFailReason",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherOutputFailReason",
                table: "AnalysisTestStudent",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailingInputFailReason",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "PrepareOtputsFailReason",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "PrepareOtputsProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "StudentOutputFailReason",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "TeacherOutputFailReason",
                table: "AnalysisTestStudent");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class addanalysisevaluationprogressindicators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailingInputEvaluationProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentOutputEvaluationProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeacherOutputEvaluationProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailingInputEvaluationProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "StudentOutputEvaluationProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "TeacherOutputEvaluationProgress",
                table: "AnalysisTestStudent");
        }
    }
}

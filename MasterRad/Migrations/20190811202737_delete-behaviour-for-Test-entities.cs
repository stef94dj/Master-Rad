using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class deletebehaviourforTestentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisTestStudent_AnalysisTest_AnalysisTestId",
                table: "AnalysisTestStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_SynthesisTestStudent_SynthesisTest_SynthesisTestId",
                table: "SynthesisTestStudent");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisTestStudent_AnalysisTest_AnalysisTestId",
                table: "AnalysisTestStudent",
                column: "AnalysisTestId",
                principalTable: "AnalysisTest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SynthesisTestStudent_SynthesisTest_SynthesisTestId",
                table: "SynthesisTestStudent",
                column: "SynthesisTestId",
                principalTable: "SynthesisTest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisTestStudent_AnalysisTest_AnalysisTestId",
                table: "AnalysisTestStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_SynthesisTestStudent_SynthesisTest_SynthesisTestId",
                table: "SynthesisTestStudent");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisTestStudent_AnalysisTest_AnalysisTestId",
                table: "AnalysisTestStudent",
                column: "AnalysisTestId",
                principalTable: "AnalysisTest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SynthesisTestStudent_SynthesisTest_SynthesisTestId",
                table: "SynthesisTestStudent",
                column: "SynthesisTestId",
                principalTable: "SynthesisTest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class mixedcompositFKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisPaper_AnalysisTestStudent_ATS_AnalysisTestId_ATS_StudentId",
                table: "AnalysisPaper");

            migrationBuilder.DropForeignKey(
                name: "FK_SynthesisPaper_SynthesisTestStudent_STS_SynthesisTestId_STS_StudentId",
                table: "SynthesisPaper");

            migrationBuilder.DropIndex(
                name: "IX_SynthesisPaper_STS_SynthesisTestId_STS_StudentId",
                table: "SynthesisPaper");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisPaper_ATS_AnalysisTestId_ATS_StudentId",
                table: "AnalysisPaper");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisPaper_STS_StudentId_STS_SynthesisTestId",
                table: "SynthesisPaper",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisPaper_ATS_StudentId_ATS_AnalysisTestId",
                table: "AnalysisPaper",
                columns: new[] { "ATS_StudentId", "ATS_AnalysisTestId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisPaper_AnalysisTestStudent_ATS_StudentId_ATS_AnalysisTestId",
                table: "AnalysisPaper",
                columns: new[] { "ATS_StudentId", "ATS_AnalysisTestId" },
                principalTable: "AnalysisTestStudent",
                principalColumns: new[] { "StudentId", "AnalysisTestId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SynthesisPaper_SynthesisTestStudent_STS_StudentId_STS_SynthesisTestId",
                table: "SynthesisPaper",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" },
                principalTable: "SynthesisTestStudent",
                principalColumns: new[] { "StudentId", "SynthesisTestId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisPaper_AnalysisTestStudent_ATS_StudentId_ATS_AnalysisTestId",
                table: "AnalysisPaper");

            migrationBuilder.DropForeignKey(
                name: "FK_SynthesisPaper_SynthesisTestStudent_STS_StudentId_STS_SynthesisTestId",
                table: "SynthesisPaper");

            migrationBuilder.DropIndex(
                name: "IX_SynthesisPaper_STS_StudentId_STS_SynthesisTestId",
                table: "SynthesisPaper");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisPaper_ATS_StudentId_ATS_AnalysisTestId",
                table: "AnalysisPaper");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisPaper_STS_SynthesisTestId_STS_StudentId",
                table: "SynthesisPaper",
                columns: new[] { "STS_SynthesisTestId", "STS_StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisPaper_ATS_AnalysisTestId_ATS_StudentId",
                table: "AnalysisPaper",
                columns: new[] { "ATS_AnalysisTestId", "ATS_StudentId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisPaper_AnalysisTestStudent_ATS_AnalysisTestId_ATS_StudentId",
                table: "AnalysisPaper",
                columns: new[] { "ATS_AnalysisTestId", "ATS_StudentId" },
                principalTable: "AnalysisTestStudent",
                principalColumns: new[] { "StudentId", "AnalysisTestId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SynthesisPaper_SynthesisTestStudent_STS_SynthesisTestId_STS_StudentId",
                table: "SynthesisPaper",
                columns: new[] { "STS_SynthesisTestId", "STS_StudentId" },
                principalTable: "SynthesisTestStudent",
                principalColumns: new[] { "StudentId", "SynthesisTestId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}

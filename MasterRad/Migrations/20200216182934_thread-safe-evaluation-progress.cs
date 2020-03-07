using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class threadsafeevaluationprogress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicDataEvaluationInfo",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "PublicDataEvaluationStatus",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "SecretDataEvaluationInfo",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "SecretDataEvaluationStatus",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "FailingInputEvaluationProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "FailingInputFailReason",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "PrepareOutputsFailReason",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "PrepareOutputsProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "StudentOutputEvaluationProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "StudentOutputFailReason",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "TeacherOutputEvaluationProgress",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "TeacherOutputFailReason",
                table: "AnalysisTestStudent");

            migrationBuilder.CreateTable(
                name: "AnalysisEvaluation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ATS_AnalysisTestId = table.Column<int>(nullable: false),
                    ATS_StudentId = table.Column<int>(nullable: false),
                    Progress = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisEvaluation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisEvaluation_AnalysisTestStudent_ATS_StudentId_ATS_AnalysisTestId",
                        columns: x => new { x.ATS_StudentId, x.ATS_AnalysisTestId },
                        principalTable: "AnalysisTestStudent",
                        principalColumns: new[] { "StudentId", "AnalysisTestId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynthesisEvaluation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    STS_SynthesisTestId = table.Column<int>(nullable: false),
                    STS_StudentId = table.Column<int>(nullable: false),
                    Progress = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    IsSecretDataUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynthesisEvaluation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynthesisEvaluation_SynthesisTestStudent_STS_StudentId_STS_SynthesisTestId",
                        columns: x => new { x.STS_StudentId, x.STS_SynthesisTestId },
                        principalTable: "SynthesisTestStudent",
                        principalColumns: new[] { "StudentId", "SynthesisTestId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisEvaluation_ATS_StudentId_ATS_AnalysisTestId",
                table: "AnalysisEvaluation",
                columns: new[] { "ATS_StudentId", "ATS_AnalysisTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisEvaluation_STS_StudentId_STS_SynthesisTestId",
                table: "SynthesisEvaluation",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisEvaluation");

            migrationBuilder.DropTable(
                name: "SynthesisEvaluation");

            migrationBuilder.AddColumn<string>(
                name: "PublicDataEvaluationInfo",
                table: "SynthesisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublicDataEvaluationStatus",
                table: "SynthesisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecretDataEvaluationInfo",
                table: "SynthesisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecretDataEvaluationStatus",
                table: "SynthesisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FailingInputEvaluationProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FailingInputFailReason",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrepareOutputsFailReason",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrepareOutputsProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentOutputEvaluationProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StudentOutputFailReason",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherOutputEvaluationProgress",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TeacherOutputFailReason",
                table: "AnalysisTestStudent",
                nullable: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class removeSynthesisPaperEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisTest_SynthesisPaper_SynthesisPaperId",
                table: "AnalysisTest");

            migrationBuilder.DropTable(
                name: "SynthesisPaper");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisTest_SynthesisPaperId",
                table: "AnalysisTest");

            migrationBuilder.RenameColumn(
                name: "SynthesisPaperId",
                table: "AnalysisTest",
                newName: "STS_SynthesisTestId");

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

            migrationBuilder.AddColumn<string>(
                name: "SqlScript",
                table: "SynthesisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TakenTest",
                table: "SynthesisTestStudent",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "STS_StudentId",
                table: "AnalysisTest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisTest_STS_StudentId_STS_SynthesisTestId",
                table: "AnalysisTest",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisTest_SynthesisTestStudent_STS_StudentId_STS_SynthesisTestId",
                table: "AnalysisTest",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" },
                principalTable: "SynthesisTestStudent",
                principalColumns: new[] { "StudentId", "SynthesisTestId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisTest_SynthesisTestStudent_STS_StudentId_STS_SynthesisTestId",
                table: "AnalysisTest");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisTest_STS_StudentId_STS_SynthesisTestId",
                table: "AnalysisTest");

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
                name: "SqlScript",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "TakenTest",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "STS_StudentId",
                table: "AnalysisTest");

            migrationBuilder.RenameColumn(
                name: "STS_SynthesisTestId",
                table: "AnalysisTest",
                newName: "SynthesisPaperId");

            migrationBuilder.CreateTable(
                name: "SynthesisPaper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PublicDataEvaluationInfo = table.Column<string>(nullable: true),
                    PublicDataEvaluationStatus = table.Column<int>(nullable: false),
                    STS_StudentId = table.Column<int>(nullable: false),
                    STS_SynthesisTestId = table.Column<int>(nullable: false),
                    SecretDataEvaluationInfo = table.Column<string>(nullable: true),
                    SecretDataEvaluationStatus = table.Column<int>(nullable: false),
                    SqlScript = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynthesisPaper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynthesisPaper_SynthesisTestStudent_STS_StudentId_STS_SynthesisTestId",
                        columns: x => new { x.STS_StudentId, x.STS_SynthesisTestId },
                        principalTable: "SynthesisTestStudent",
                        principalColumns: new[] { "StudentId", "SynthesisTestId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisTest_SynthesisPaperId",
                table: "AnalysisTest",
                column: "SynthesisPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisPaper_STS_StudentId_STS_SynthesisTestId",
                table: "SynthesisPaper",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisTest_SynthesisPaper_SynthesisPaperId",
                table: "AnalysisTest",
                column: "SynthesisPaperId",
                principalTable: "SynthesisPaper",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

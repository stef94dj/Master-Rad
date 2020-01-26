using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class removeAnalysisPaperEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisPaper");

            migrationBuilder.AddColumn<string>(
                name: "StudentOutputNameOnServer",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TakenTest",
                table: "AnalysisTestStudent",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TeacherOutputNameOnServer",
                table: "AnalysisTestStudent",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentOutputNameOnServer",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "TakenTest",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "TeacherOutputNameOnServer",
                table: "AnalysisTestStudent");

            migrationBuilder.CreateTable(
                name: "AnalysisPaper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ATS_AnalysisTestId = table.Column<int>(nullable: false),
                    ATS_StudentId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisPaper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisPaper_AnalysisTestStudent_ATS_StudentId_ATS_AnalysisTestId",
                        columns: x => new { x.ATS_StudentId, x.ATS_AnalysisTestId },
                        principalTable: "AnalysisTestStudent",
                        principalColumns: new[] { "StudentId", "AnalysisTestId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisPaper_ATS_StudentId_ATS_AnalysisTestId",
                table: "AnalysisPaper",
                columns: new[] { "ATS_StudentId", "ATS_AnalysisTestId" },
                unique: true);
        }
    }
}

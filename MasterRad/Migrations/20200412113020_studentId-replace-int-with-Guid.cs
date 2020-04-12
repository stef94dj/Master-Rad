using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class studentIdreplaceintwithGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisTestStudent_Student_StudentId",
                table: "AnalysisTestStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_SynthesisTestStudent_Student_StudentId",
                table: "SynthesisTestStudent");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "SynthesisTestStudent",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<Guid>(
                name: "STS_StudentId",
                table: "SynthesisEvaluation",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "AnalysisTestStudent",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<Guid>(
                name: "STS_StudentId",
                table: "AnalysisTest",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<Guid>(
                name: "ATS_StudentId",
                table: "AnalysisEvaluation",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "AzureSqlUserMap",
                columns: table => new
                {
                    AzureId = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    SqlUsername = table.Column<string>(maxLength: 255, nullable: false),
                    SqlPassword = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AzureSqlUserMap", x => x.AzureId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisTestStudent_AzureSqlUserMap_StudentId",
                table: "AnalysisTestStudent",
                column: "StudentId",
                principalTable: "AzureSqlUserMap",
                principalColumn: "AzureId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SynthesisTestStudent_AzureSqlUserMap_StudentId",
                table: "SynthesisTestStudent",
                column: "StudentId",
                principalTable: "AzureSqlUserMap",
                principalColumn: "AzureId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisTestStudent_AzureSqlUserMap_StudentId",
                table: "AnalysisTestStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_SynthesisTestStudent_AzureSqlUserMap_StudentId",
                table: "SynthesisTestStudent");

            migrationBuilder.DropTable(
                name: "AzureSqlUserMap");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "SynthesisTestStudent",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "STS_StudentId",
                table: "SynthesisEvaluation",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "AnalysisTestStudent",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "STS_StudentId",
                table: "AnalysisTest",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "ATS_StudentId",
                table: "AnalysisEvaluation",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(maxLength: 255, nullable: true),
                    LastName = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "DateCreated", "Email", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, null, "stud1@student.etf.bg.ac.rs", "John", "Smith" },
                    { 2, null, "stud2@student.etf.bg.ac.rs", "Jane", "Rogers" },
                    { 3, null, "stud3@student.etf.bg.ac.rs", "John", "Rogers" },
                    { 4, null, "stud4@student.etf.bg.ac.rs", "Jane", "Smith" },
                    { 5, null, "stud5@student.etf.bg.ac.rs", null, null },
                    { 6, null, "stud6@student.etf.bg.ac.rs", null, null },
                    { 7, null, "stud7@student.etf.bg.ac.rs", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Email",
                table: "Student",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisTestStudent_Student_StudentId",
                table: "AnalysisTestStudent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SynthesisTestStudent_Student_StudentId",
                table: "SynthesisTestStudent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

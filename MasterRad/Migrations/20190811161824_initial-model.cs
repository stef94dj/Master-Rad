using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class initialmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    ModelDescription = table.Column<string>(nullable: true),
                    NameOnServer = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnhandledExceptionLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    LogMethod = table.Column<int>(nullable: false),
                    SerializeError = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Headers = table.Column<string>(nullable: true),
                    Cookies = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    PathBase = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    Protocol = table.Column<string>(nullable: true),
                    QueryString = table.Column<string>(nullable: true),
                    Query = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnhandledExceptionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DbTemplateId = table.Column<int>(nullable: false),
                    TemplateId = table.Column<int>(nullable: true),
                    SolutionSqlScript = table.Column<string>(nullable: true),
                    IsDataSet = table.Column<bool>(nullable: false),
                    NameOnServer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_DbTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "DbTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolutionColumn",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: false),
                    ColumnName = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionColumn_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynthesisTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    TaskId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynthesisTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynthesisTest_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SynthesisTestStudent",
                columns: table => new
                {
                    SynthesisTestId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynthesisTestStudent", x => new { x.StudentId, x.SynthesisTestId });
                    table.ForeignKey(
                        name: "FK_SynthesisTestStudent_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SynthesisTestStudent_SynthesisTest_SynthesisTestId",
                        column: x => x.SynthesisTestId,
                        principalTable: "SynthesisTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynthesisPaper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    SqlScript = table.Column<string>(nullable: true),
                    STS_SynthesisTestId = table.Column<int>(nullable: false),
                    STS_StudentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynthesisPaper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynthesisPaper_SynthesisTestStudent_STS_SynthesisTestId_STS_StudentId",
                        columns: x => new { x.STS_SynthesisTestId, x.STS_StudentId },
                        principalTable: "SynthesisTestStudent",
                        principalColumns: new[] { "StudentId", "SynthesisTestId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynthesisPaperStudent",
                columns: table => new
                {
                    StudentId = table.Column<int>(nullable: false),
                    SynthesisPaperId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynthesisPaperStudent", x => new { x.StudentId, x.SynthesisPaperId });
                    table.ForeignKey(
                        name: "FK_SynthesisPaperStudent_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SynthesisPaperStudent_SynthesisPaper_SynthesisPaperId",
                        column: x => x.SynthesisPaperId,
                        principalTable: "SynthesisPaper",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisPaper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    SqlScript = table.Column<string>(nullable: true),
                    SPS_SynthesisPaperId = table.Column<int>(nullable: false),
                    SPS_StudentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisPaper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisPaper_SynthesisPaperStudent_SPS_SynthesisPaperId_SPS_StudentId",
                        columns: x => new { x.SPS_SynthesisPaperId, x.SPS_StudentId },
                        principalTable: "SynthesisPaperStudent",
                        principalColumns: new[] { "StudentId", "SynthesisPaperId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisPaper_SPS_SynthesisPaperId_SPS_StudentId",
                table: "AnalysisPaper",
                columns: new[] { "SPS_SynthesisPaperId", "SPS_StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionColumn_TaskId",
                table: "SolutionColumn",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisPaper_STS_SynthesisTestId_STS_StudentId",
                table: "SynthesisPaper",
                columns: new[] { "STS_SynthesisTestId", "STS_StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisPaperStudent_SynthesisPaperId",
                table: "SynthesisPaperStudent",
                column: "SynthesisPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisTest_TaskId",
                table: "SynthesisTest",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisTestStudent_SynthesisTestId",
                table: "SynthesisTestStudent",
                column: "SynthesisTestId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_TemplateId",
                table: "Task",
                column: "TemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisPaper");

            migrationBuilder.DropTable(
                name: "SolutionColumn");

            migrationBuilder.DropTable(
                name: "UnhandledExceptionLog");

            migrationBuilder.DropTable(
                name: "SynthesisPaperStudent");

            migrationBuilder.DropTable(
                name: "SynthesisPaper");

            migrationBuilder.DropTable(
                name: "SynthesisTestStudent");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "SynthesisTest");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "DbTemplate");
        }
    }
}

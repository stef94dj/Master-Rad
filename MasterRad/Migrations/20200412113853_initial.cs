using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "ExceptionLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    LogMethod = table.Column<int>(nullable: false),
                    SerializeError = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Template",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    ModelDescription = table.Column<string>(maxLength: 8191, nullable: true),
                    NameOnServer = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Template", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Headers = table.Column<string>(nullable: true),
                    Cookies = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    PathBase = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    Protocol = table.Column<string>(nullable: true),
                    QueryString = table.Column<string>(nullable: true),
                    Query = table.Column<string>(nullable: true),
                    ExceptionLogId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestLog_ExceptionLog_ExceptionLogId",
                        column: x => x.ExceptionLogId,
                        principalTable: "ExceptionLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 8191, nullable: true),
                    NameOnServer = table.Column<string>(maxLength: 255, nullable: true),
                    TemplateId = table.Column<int>(nullable: false),
                    SolutionSqlScript = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_Template_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolutionColumn",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: false),
                    ColumnName = table.Column<string>(maxLength: 255, nullable: false),
                    SqlType = table.Column<string>(maxLength: 50, nullable: false)
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<int>(nullable: false),
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
                    StudentId = table.Column<Guid>(nullable: false),
                    SynthesisTestId = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    NameOnServer = table.Column<string>(maxLength: 255, nullable: false),
                    TakenTest = table.Column<bool>(nullable: false),
                    SqlScript = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynthesisTestStudent", x => new { x.StudentId, x.SynthesisTestId });
                    table.ForeignKey(
                        name: "FK_SynthesisTestStudent_AzureSqlUserMap_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AzureSqlUserMap",
                        principalColumn: "AzureId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SynthesisTestStudent_SynthesisTest_SynthesisTestId",
                        column: x => x.SynthesisTestId,
                        principalTable: "SynthesisTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    STS_SynthesisTestId = table.Column<int>(nullable: false),
                    STS_StudentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisTest_SynthesisTestStudent_STS_StudentId_STS_SynthesisTestId",
                        columns: x => new { x.STS_StudentId, x.STS_SynthesisTestId },
                        principalTable: "SynthesisTestStudent",
                        principalColumns: new[] { "StudentId", "SynthesisTestId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SynthesisEvaluation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    STS_SynthesisTestId = table.Column<int>(nullable: false),
                    STS_StudentId = table.Column<Guid>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "AnalysisTestStudent",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(nullable: false),
                    AnalysisTestId = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TakenTest = table.Column<bool>(nullable: false),
                    InputNameOnServer = table.Column<string>(maxLength: 255, nullable: false),
                    TeacherOutputNameOnServer = table.Column<string>(nullable: true),
                    StudentOutputNameOnServer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisTestStudent", x => new { x.StudentId, x.AnalysisTestId });
                    table.ForeignKey(
                        name: "FK_AnalysisTestStudent_AnalysisTest_AnalysisTestId",
                        column: x => x.AnalysisTestId,
                        principalTable: "AnalysisTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnalysisTestStudent_AzureSqlUserMap_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AzureSqlUserMap",
                        principalColumn: "AzureId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisEvaluation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ATS_AnalysisTestId = table.Column<int>(nullable: false),
                    ATS_StudentId = table.Column<Guid>(nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisEvaluation_ATS_StudentId_ATS_AnalysisTestId",
                table: "AnalysisEvaluation",
                columns: new[] { "ATS_StudentId", "ATS_AnalysisTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisTest_Name",
                table: "AnalysisTest",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisTest_STS_StudentId_STS_SynthesisTestId",
                table: "AnalysisTest",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisTestStudent_AnalysisTestId",
                table: "AnalysisTestStudent",
                column: "AnalysisTestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLog_ExceptionLogId",
                table: "RequestLog",
                column: "ExceptionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionColumn_TaskId",
                table: "SolutionColumn",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisEvaluation_STS_StudentId_STS_SynthesisTestId",
                table: "SynthesisEvaluation",
                columns: new[] { "STS_StudentId", "STS_SynthesisTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisTest_Name",
                table: "SynthesisTest",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisTest_TaskId",
                table: "SynthesisTest",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SynthesisTestStudent_SynthesisTestId",
                table: "SynthesisTestStudent",
                column: "SynthesisTestId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Name",
                table: "Task",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_TemplateId",
                table: "Task",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Template_Name",
                table: "Template",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisEvaluation");

            migrationBuilder.DropTable(
                name: "RequestLog");

            migrationBuilder.DropTable(
                name: "SolutionColumn");

            migrationBuilder.DropTable(
                name: "SynthesisEvaluation");

            migrationBuilder.DropTable(
                name: "AnalysisTestStudent");

            migrationBuilder.DropTable(
                name: "ExceptionLog");

            migrationBuilder.DropTable(
                name: "AnalysisTest");

            migrationBuilder.DropTable(
                name: "SynthesisTestStudent");

            migrationBuilder.DropTable(
                name: "AzureSqlUserMap");

            migrationBuilder.DropTable(
                name: "SynthesisTest");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Template");
        }
    }
}

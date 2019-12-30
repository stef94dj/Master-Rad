using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class fixExceptionLogRequestLogrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExceptionLog_RequestLog_RequestId",
                table: "ExceptionLog");

            migrationBuilder.DropIndex(
                name: "IX_ExceptionLog_RequestId",
                table: "ExceptionLog");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "ExceptionLog");

            migrationBuilder.RenameColumn(
                name: "LogReason",
                table: "RequestLog",
                newName: "ExceptionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLog_ExceptionLogId",
                table: "RequestLog",
                column: "ExceptionLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestLog_ExceptionLog_ExceptionLogId",
                table: "RequestLog",
                column: "ExceptionLogId",
                principalTable: "ExceptionLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestLog_ExceptionLog_ExceptionLogId",
                table: "RequestLog");

            migrationBuilder.DropIndex(
                name: "IX_RequestLog_ExceptionLogId",
                table: "RequestLog");

            migrationBuilder.RenameColumn(
                name: "ExceptionLogId",
                table: "RequestLog",
                newName: "LogReason");

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "ExceptionLog",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionLog_RequestId",
                table: "ExceptionLog",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExceptionLog_RequestLog_RequestId",
                table: "ExceptionLog",
                column: "RequestId",
                principalTable: "RequestLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

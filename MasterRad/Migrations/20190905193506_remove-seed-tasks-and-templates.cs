using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class removeseedtasksandtemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Task",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Template",
                keyColumn: "Id",
                keyValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Template",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateModified", "ModelDescription", "ModifiedBy", "Name", "NameOnServer" },
                values: new object[] { 1, null, null, null, null, null, "template", "Tmp_template" });

            migrationBuilder.InsertData(
                table: "Task",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateModified", "Description", "ModifiedBy", "Name", "NameOnServer", "SolutionSqlScript", "TemplateId" },
                values: new object[] { 1, null, null, null, null, null, "task", "Tsk_task", null, 1 });
        }
    }
}

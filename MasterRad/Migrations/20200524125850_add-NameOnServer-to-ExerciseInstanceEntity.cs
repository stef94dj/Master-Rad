using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class addNameOnServertoExerciseInstanceEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameOnServer",
                table: "ExerciseInstance",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseInstance_NameOnServer",
                table: "ExerciseInstance",
                column: "NameOnServer",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExerciseInstance_NameOnServer",
                table: "ExerciseInstance");

            migrationBuilder.DropColumn(
                name: "NameOnServer",
                table: "ExerciseInstance");
        }
    }
}

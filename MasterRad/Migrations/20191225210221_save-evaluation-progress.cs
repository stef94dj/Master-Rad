using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class saveevaluationprogress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassPublicTest",
                table: "SynthesisPaper");

            migrationBuilder.DropColumn(
                name: "PassSecretTest",
                table: "SynthesisPaper");

            migrationBuilder.RenameColumn(
                name: "SecretTestFailReason",
                table: "SynthesisPaper",
                newName: "SecretDataEvaluationInfo");

            migrationBuilder.RenameColumn(
                name: "PublicTestFailReason",
                table: "SynthesisPaper",
                newName: "PublicDataEvaluationInfo");

            migrationBuilder.AddColumn<int>(
                name: "PublicDataEvaluationStatus",
                table: "SynthesisPaper",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecretDataEvaluationStatus",
                table: "SynthesisPaper",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicDataEvaluationStatus",
                table: "SynthesisPaper");

            migrationBuilder.DropColumn(
                name: "SecretDataEvaluationStatus",
                table: "SynthesisPaper");

            migrationBuilder.RenameColumn(
                name: "SecretDataEvaluationInfo",
                table: "SynthesisPaper",
                newName: "SecretTestFailReason");

            migrationBuilder.RenameColumn(
                name: "PublicDataEvaluationInfo",
                table: "SynthesisPaper",
                newName: "PublicTestFailReason");

            migrationBuilder.AddColumn<bool>(
                name: "PassPublicTest",
                table: "SynthesisPaper",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PassSecretTest",
                table: "SynthesisPaper",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class synthesisevaluationresult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PassPublicTest",
                table: "SynthesisPaper",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PassSecretTest",
                table: "SynthesisPaper",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicTestFailReason",
                table: "SynthesisPaper",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretTestFailReason",
                table: "SynthesisPaper",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassPublicTest",
                table: "SynthesisPaper");

            migrationBuilder.DropColumn(
                name: "PassSecretTest",
                table: "SynthesisPaper");

            migrationBuilder.DropColumn(
                name: "PublicTestFailReason",
                table: "SynthesisPaper");

            migrationBuilder.DropColumn(
                name: "SecretTestFailReason",
                table: "SynthesisPaper");
        }
    }
}

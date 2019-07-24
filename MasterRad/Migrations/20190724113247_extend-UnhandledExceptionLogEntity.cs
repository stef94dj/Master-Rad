using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class extendUnhandledExceptionLogEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogMethod",
                table: "UnhandledExceptionLog",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SerializeError",
                table: "UnhandledExceptionLog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogMethod",
                table: "UnhandledExceptionLog");

            migrationBuilder.DropColumn(
                name: "SerializeError",
                table: "UnhandledExceptionLog");
        }
    }
}

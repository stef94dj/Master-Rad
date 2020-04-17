using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class adduniqueonSqlUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AzureSqlUserMap_SqlUsername",
                table: "AzureSqlUserMap",
                column: "SqlUsername",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AzureSqlUserMap_SqlUsername",
                table: "AzureSqlUserMap");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class modifieddbtemplateandaddedtaskentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DbName",
                table: "DbTemplate",
                newName: "ModelDescription");

            migrationBuilder.AddColumn<bool>(
                name: "IsBaseDataSet",
                table: "DbTemplate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DbTemplate",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameOnServer",
                table: "DbTemplate",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DbTemplateId = table.Column<int>(nullable: false),
                    IsDataSet = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_DbTemplate_DbTemplateId",
                        column: x => x.DbTemplateId,
                        principalTable: "DbTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Task_DbTemplateId",
                table: "Task",
                column: "DbTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropColumn(
                name: "IsBaseDataSet",
                table: "DbTemplate");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DbTemplate");

            migrationBuilder.DropColumn(
                name: "NameOnServer",
                table: "DbTemplate");

            migrationBuilder.RenameColumn(
                name: "ModelDescription",
                table: "DbTemplate",
                newName: "DbName");
        }
    }
}

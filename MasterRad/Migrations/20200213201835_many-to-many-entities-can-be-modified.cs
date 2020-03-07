using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class manytomanyentitiescanbemodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "SynthesisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "SynthesisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "AnalysisTestStudent",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "AnalysisTestStudent");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterRad.Migrations
{
    public partial class baseStudentTestEntityaddecolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SynthesisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "SynthesisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "TimeStamp",
                table: "SynthesisTestStudent",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AnalysisTestStudent",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "TimeStamp",
                table: "AnalysisTestStudent",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "SynthesisTestStudent");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AnalysisTestStudent");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "AnalysisTestStudent");
        }
    }
}

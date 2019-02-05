using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class TournamentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Tournaments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Tournaments");
        }
    }
}

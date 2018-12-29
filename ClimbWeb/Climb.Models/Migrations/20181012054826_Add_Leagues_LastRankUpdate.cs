using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_Leagues_LastRankUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRankUpdate",
                table: "Leagues",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRankUpdate",
                table: "Leagues");
        }
    }
}

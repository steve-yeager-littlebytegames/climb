using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class AddLeagueUser_IsNewcomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNewcomer",
                table: "LeagueUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNewcomer",
                table: "LeagueUsers");
        }
    }
}

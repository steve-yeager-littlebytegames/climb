using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_LeagueUser_RankTrend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RankTrend",
                table: "LeagueUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RankTrend",
                table: "LeagueUsers");
        }
    }
}

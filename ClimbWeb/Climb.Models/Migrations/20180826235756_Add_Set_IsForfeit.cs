using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_Set_IsForfeit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForfeit",
                table: "Sets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasLeft",
                table: "SeasonLeagueUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "SeasonLeagueUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonLeagueUsers_UserID",
                table: "SeasonLeagueUsers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonLeagueUsers_AspNetUsers_UserID",
                table: "SeasonLeagueUsers",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonLeagueUsers_AspNetUsers_UserID",
                table: "SeasonLeagueUsers");

            migrationBuilder.DropIndex(
                name: "IX_SeasonLeagueUsers_UserID",
                table: "SeasonLeagueUsers");

            migrationBuilder.DropColumn(
                name: "IsForfeit",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "HasLeft",
                table: "SeasonLeagueUsers");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "SeasonLeagueUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class AddLeague_ActiveSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveSeasonID",
                table: "Leagues",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_ActiveSeasonID",
                table: "Leagues",
                column: "ActiveSeasonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leagues_Seasons_ActiveSeasonID",
                table: "Leagues",
                column: "ActiveSeasonID",
                principalTable: "Seasons",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leagues_Seasons_ActiveSeasonID",
                table: "Leagues");

            migrationBuilder.DropIndex(
                name: "IX_Leagues_ActiveSeasonID",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ActiveSeasonID",
                table: "Leagues");
        }
    }
}

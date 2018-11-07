using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_Tournament_TournamentUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TournamentID",
                table: "TournamentUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_TournamentID",
                table: "TournamentUsers",
                column: "TournamentID");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentUsers_Tournaments_TournamentID",
                table: "TournamentUsers",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentUsers_Tournaments_TournamentID",
                table: "TournamentUsers");

            migrationBuilder.DropIndex(
                name: "IX_TournamentUsers_TournamentID",
                table: "TournamentUsers");

            migrationBuilder.DropColumn(
                name: "TournamentID",
                table: "TournamentUsers");
        }
    }
}

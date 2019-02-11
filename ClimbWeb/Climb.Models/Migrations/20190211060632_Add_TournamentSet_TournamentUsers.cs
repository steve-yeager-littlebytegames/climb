using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_TournamentSet_TournamentUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TournamentPlayer1ID",
                table: "Sets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TournamentPlayer2ID",
                table: "Sets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TournamentUser1ID",
                table: "Sets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TournamentUser2ID",
                table: "Sets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sets_TournamentPlayer1ID",
                table: "Sets",
                column: "TournamentPlayer1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_TournamentPlayer2ID",
                table: "Sets",
                column: "TournamentPlayer2ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_TournamentUsers_TournamentPlayer1ID",
                table: "Sets",
                column: "TournamentPlayer1ID",
                principalTable: "TournamentUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_TournamentUsers_TournamentPlayer2ID",
                table: "Sets",
                column: "TournamentPlayer2ID",
                principalTable: "TournamentUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TournamentUsers_TournamentPlayer1ID",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TournamentUsers_TournamentPlayer2ID",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Sets_TournamentPlayer1ID",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Sets_TournamentPlayer2ID",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "TournamentPlayer1ID",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "TournamentPlayer2ID",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "TournamentUser1ID",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "TournamentUser2ID",
                table: "Sets");
        }
    }
}

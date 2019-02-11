using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_SetSlot_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "User1ID",
                table: "SetSlots",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "User2ID",
                table: "SetSlots",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_User1ID",
                table: "SetSlots",
                column: "User1ID");

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_User2ID",
                table: "SetSlots",
                column: "User2ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SetSlots_TournamentUsers_User1ID",
                table: "SetSlots",
                column: "User1ID",
                principalTable: "TournamentUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SetSlots_TournamentUsers_User2ID",
                table: "SetSlots",
                column: "User2ID",
                principalTable: "TournamentUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetSlots_TournamentUsers_User1ID",
                table: "SetSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_SetSlots_TournamentUsers_User2ID",
                table: "SetSlots");

            migrationBuilder.DropIndex(
                name: "IX_SetSlots_User1ID",
                table: "SetSlots");

            migrationBuilder.DropIndex(
                name: "IX_SetSlots_User2ID",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "User1ID",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "User2ID",
                table: "SetSlots");
        }
    }
}

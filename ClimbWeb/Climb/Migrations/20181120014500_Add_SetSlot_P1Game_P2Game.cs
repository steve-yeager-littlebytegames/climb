using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_SetSlot_P1Game_P2Game : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "P1Game",
                table: "SetSlots",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "P2Game",
                table: "SetSlots",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "P1Game",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "P2Game",
                table: "SetSlots");
        }
    }
}

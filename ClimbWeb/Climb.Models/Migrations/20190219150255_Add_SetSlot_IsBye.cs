using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_SetSlot_IsBye : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBye",
                table: "SetSlots",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBye",
                table: "SetSlots");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Rename_Set_SetSlotID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetSlots_Sets_SetID",
                table: "SetSlots");

            migrationBuilder.DropIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "SetID",
                table: "SetSlots");

            migrationBuilder.RenameColumn(
                name: "SlotID",
                table: "Sets",
                newName: "SetSlotID");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_SetSlotID",
                table: "Sets",
                column: "SetSlotID",
                unique: true,
                filter: "[SetSlotID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_SetSlots_SetSlotID",
                table: "Sets",
                column: "SetSlotID",
                principalTable: "SetSlots",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_SetSlots_SetSlotID",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Sets_SetSlotID",
                table: "Sets");

            migrationBuilder.RenameColumn(
                name: "SetSlotID",
                table: "Sets",
                newName: "SlotID");

            migrationBuilder.AddColumn<int>(
                name: "SetID",
                table: "SetSlots",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots",
                column: "SetID",
                unique: true,
                filter: "[SetID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SetSlots_Sets_SetID",
                table: "SetSlots",
                column: "SetID",
                principalTable: "Sets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

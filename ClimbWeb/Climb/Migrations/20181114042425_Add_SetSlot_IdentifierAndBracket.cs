using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_SetSlot_IdentifierAndBracket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "SetSlots");

            migrationBuilder.RenameColumn(
                name: "WinSlotID",
                table: "SetSlots",
                newName: "WinSlotIdentifier");

            migrationBuilder.RenameColumn(
                name: "LoseSlotID",
                table: "SetSlots",
                newName: "LoseSlotIdentifier");

            migrationBuilder.AddColumn<int>(
                name: "Identifier",
                table: "SetSlots",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Bracket",
                table: "SetSlots",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots",
                columns: new[] { "Identifier", "TournamentID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "Bracket",
                table: "SetSlots");

            migrationBuilder.RenameColumn(
                name: "WinSlotIdentifier",
                table: "SetSlots",
                newName: "WinSlotID");

            migrationBuilder.RenameColumn(
                name: "LoseSlotIdentifier",
                table: "SetSlots",
                newName: "LoseSlotID");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "SetSlots",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots",
                column: "ID");
        }
    }
}

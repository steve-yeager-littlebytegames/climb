using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_SetSlot_ID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots");

            migrationBuilder.DropIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "IsBye",
                table: "SetSlots");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "SetSlots",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "SlotID",
                table: "Sets",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots",
                column: "SetID",
                unique: true,
                filter: "[SetID] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots");

            migrationBuilder.DropIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "SlotID",
                table: "Sets");

            migrationBuilder.AddColumn<bool>(
                name: "IsBye",
                table: "SetSlots",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetSlots",
                table: "SetSlots",
                columns: new[] { "Identifier", "TournamentID" });

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots",
                column: "SetID");
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_Rounds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoundName",
                table: "SetSlots");

            migrationBuilder.RenameColumn(
                name: "Bracket",
                table: "SetSlots",
                newName: "RoundID");

            migrationBuilder.AddColumn<bool>(
                name: "IsBye",
                table: "SetSlots",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SetID",
                table: "SetSlots",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TournamentID = table.Column<int>(nullable: false),
                    Bracket = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rounds_Tournaments_TournamentID",
                        column: x => x.TournamentID,
                        principalTable: "Tournaments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_RoundID",
                table: "SetSlots",
                column: "RoundID");

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_TournamentID",
                table: "Rounds",
                column: "TournamentID");

            migrationBuilder.AddForeignKey(
                name: "FK_SetSlots_Rounds_RoundID",
                table: "SetSlots",
                column: "RoundID",
                principalTable: "Rounds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SetSlots_Sets_SetID",
                table: "SetSlots",
                column: "SetID",
                principalTable: "Sets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetSlots_Rounds_RoundID",
                table: "SetSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_SetSlots_Sets_SetID",
                table: "SetSlots");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_SetSlots_RoundID",
                table: "SetSlots");

            migrationBuilder.DropIndex(
                name: "IX_SetSlots_SetID",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "IsBye",
                table: "SetSlots");

            migrationBuilder.DropColumn(
                name: "SetID",
                table: "SetSlots");

            migrationBuilder.RenameColumn(
                name: "RoundID",
                table: "SetSlots",
                newName: "Bracket");

            migrationBuilder.AddColumn<string>(
                name: "RoundName",
                table: "SetSlots",
                nullable: true);
        }
    }
}

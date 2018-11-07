using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_Tournament_Models : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TournamentID",
                table: "Sets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TournamentID",
                table: "LeagueUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    LeagueID = table.Column<int>(nullable: false),
                    SeasonID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tournaments_Leagues_LeagueID",
                        column: x => x.LeagueID,
                        principalTable: "Leagues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tournaments_Seasons_SeasonID",
                        column: x => x.SeasonID,
                        principalTable: "Seasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TournamentUsers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<string>(nullable: true),
                    LeagueUserID = table.Column<int>(nullable: false),
                    SeasonLeagueUserID = table.Column<int>(nullable: true),
                    Seed = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TournamentUsers_LeagueUsers_LeagueUserID",
                        column: x => x.LeagueUserID,
                        principalTable: "LeagueUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentUsers_SeasonLeagueUsers_SeasonLeagueUserID",
                        column: x => x.SeasonLeagueUserID,
                        principalTable: "SeasonLeagueUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentUsers_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetSlots",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TournamentID = table.Column<int>(nullable: false),
                    RoundName = table.Column<string>(nullable: true),
                    WinSlotID = table.Column<int>(nullable: true),
                    LoseSlotID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetSlots", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SetSlots_Tournaments_TournamentID",
                        column: x => x.TournamentID,
                        principalTable: "Tournaments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sets_TournamentID",
                table: "Sets",
                column: "TournamentID");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUsers_TournamentID",
                table: "LeagueUsers",
                column: "TournamentID");

            migrationBuilder.CreateIndex(
                name: "IX_SetSlots_TournamentID",
                table: "SetSlots",
                column: "TournamentID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_LeagueID",
                table: "Tournaments",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_SeasonID",
                table: "Tournaments",
                column: "SeasonID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_LeagueUserID",
                table: "TournamentUsers",
                column: "LeagueUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_SeasonLeagueUserID",
                table: "TournamentUsers",
                column: "SeasonLeagueUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentUsers_UserID",
                table: "TournamentUsers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUsers_Tournaments_TournamentID",
                table: "LeagueUsers",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_Tournaments_TournamentID",
                table: "Sets",
                column: "TournamentID",
                principalTable: "Tournaments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUsers_Tournaments_TournamentID",
                table: "LeagueUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_Tournaments_TournamentID",
                table: "Sets");

            migrationBuilder.DropTable(
                name: "SetSlots");

            migrationBuilder.DropTable(
                name: "TournamentUsers");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Sets_TournamentID",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_LeagueUsers_TournamentID",
                table: "LeagueUsers");

            migrationBuilder.DropColumn(
                name: "TournamentID",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "TournamentID",
                table: "LeagueUsers");
        }
    }
}

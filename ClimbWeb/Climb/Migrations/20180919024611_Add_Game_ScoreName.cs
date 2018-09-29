using Microsoft.EntityFrameworkCore.Migrations;

namespace Climb.Migrations
{
    public partial class Add_Game_ScoreName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasStages",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Games",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "ScoreName",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreName",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Games",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasStages",
                table: "Games",
                nullable: false,
                defaultValue: false);
        }
    }
}

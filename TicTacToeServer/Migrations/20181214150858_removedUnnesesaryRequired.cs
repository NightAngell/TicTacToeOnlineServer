using Microsoft.EntityFrameworkCore.Migrations;

namespace TicTacToeServer.Migrations
{
    public partial class removedUnnesesaryRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CurrentPlayerId",
                table: "Game",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CurrentPlayerId",
                table: "Game",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

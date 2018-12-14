using Microsoft.EntityFrameworkCore.Migrations;

namespace TicTacToeServer.Migrations
{
    public partial class AddNullableGameIdAndAccesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Rooms",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Rooms",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

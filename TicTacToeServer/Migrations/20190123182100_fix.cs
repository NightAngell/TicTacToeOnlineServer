using Microsoft.EntityFrameworkCore.Migrations;

namespace TicTacToeServer.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_GameField_FieldId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_GameId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Game_FieldId",
                table: "Game");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "GameField",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GameField_GameId",
                table: "GameField",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game_RoomId",
                table: "Game",
                column: "RoomId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Rooms_RoomId",
                table: "Game",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameField_Game_GameId",
                table: "GameField",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Rooms_RoomId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_GameField_Game_GameId",
                table: "GameField");

            migrationBuilder.DropIndex(
                name: "IX_GameField_GameId",
                table: "GameField");

            migrationBuilder.DropIndex(
                name: "IX_Game_RoomId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GameField");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Game");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_GameId",
                table: "Rooms",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_FieldId",
                table: "Game",
                column: "FieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_GameField_FieldId",
                table: "Game",
                column: "FieldId",
                principalTable: "GameField",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

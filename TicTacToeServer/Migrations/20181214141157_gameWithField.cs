using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TicTacToeServer.Migrations
{
    public partial class gameWithField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HostNick",
                table: "Rooms",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Rooms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GameField",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TopLeft = table.Column<string>(nullable: true),
                    Top = table.Column<string>(nullable: true),
                    TopRight = table.Column<string>(nullable: true),
                    MiddleLeft = table.Column<string>(nullable: true),
                    Middle = table.Column<string>(nullable: true),
                    MiddleRight = table.Column<string>(nullable: true),
                    DownLeft = table.Column<string>(nullable: true),
                    Down = table.Column<string>(nullable: true),
                    DownRight = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameField", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldId = table.Column<int>(nullable: false),
                    CurrentPlayerId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_GameField_FieldId",
                        column: x => x.FieldId,
                        principalTable: "GameField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_GameId",
                table: "Rooms",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_FieldId",
                table: "Game",
                column: "FieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Game_GameId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "GameField");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_GameId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "HostNick",
                table: "Rooms",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TicTacToeServer.Enums;
using TicTacToeServer.Exceptions;
using TicTacToeServer.Models;

namespace TicTacToeServer.Services
{
    public interface IGameService
    {
        bool ValidatePlayer(Room room, string playerId, string password);
        bool IsWinner(GameField field);
        bool IsPlayerTurn(Game game, string playerId);
        bool CanMakeMove(GameField field, GameFieldFields simpleField);
        GameFieldFields Map2DimensionalParamsToGameSimpleField(int i, int j);
        void MakeMove(Game game, GameFieldFields simpleField);
        void NextPlayerTurn(Room room);
        bool IsWinnerUnderTheSlant(GameField field);
        bool IsWinnerHorizontal(GameField field);
        bool IsWinnerVertical(GameField field);
        Task SaveChangesAsync();
    }
}
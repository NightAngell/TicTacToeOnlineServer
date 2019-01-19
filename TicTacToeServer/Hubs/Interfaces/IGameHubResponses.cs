using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.Hubs.Interfaces
{
    public interface IGameHubResponses : IHubResponsesConstraint
    {
        Task OpponentJoinedToGame();
        Task AllPlayersJoinedToRoom();
        Task NotYourTurn(string message);
        Task FieldAlreadyOccupied(string message);
        Task Win();
        Task Lose();
        Task Draw();
        Task PlayerMadeMove(int i, int j);
        Task OpponentDisconnected();
        Task AccesDenied();
        Task RoomNotExist();
    }
}

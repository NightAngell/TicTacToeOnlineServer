using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TicTacToeServer.Services
{
    public interface IGameService
    {
        Task<bool> ValidatePlayer(int gameId, string playerId, string password, Hub gameHub);
    }
}
using Microsoft.AspNetCore.SignalR;

namespace TicTacToeServer.Services
{
    public interface IHubService
    {
        void AddOrUpdateItemInContextItems<T>(Hub hub, string key, T value);
    }
}
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinToGame(int gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _createGroupName(gameId));
            Context.Items.Add("gameId", gameId);
            await Clients.Caller.SendAsync("JoinedToGame");
        }

        public async Task Ping(int gameId)
        {
            await Clients.OthersInGroup(_createGroupName(gameId)).SendAsync("OpponentConnected");
        }

        public async Task MakeMove(int i, int j)
        {
            await Clients
                .OthersInGroup(_createGroupName((int)Context.Items["gameId"]))
                .SendAsync("OpponentMadeMove", i, j);
        }

        private string _createGroupName(int gameId)
        {
            var now = DateTime.Now;
            var groupName = $"{now.Year}/{gameId}";
            return groupName;
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Enums;
using TicTacToeServer.Services;
using TicTacToeServer.Services.Interfaces;

namespace TicTacToeServer.Hubs
{
    public class GameHub : Hub
    {
        readonly IGameService _gameService;
        readonly IRoomService _roomService;

        public GameHub(IGameService gameService, IRoomService roomService)
        {
           _gameService = gameService;
           _roomService = roomService;
        }

        public async Task JoinToGame(int gameId, string playerId, string password)
        {
            if(!await _gameService.ValidatePlayer(gameId, playerId, password, this)) {
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            Context.Items.Add("gameId", gameId);
            await Clients.OthersInGroup(gameId.ToString()).SendAsync("OpponentJoinedToGame");

            var room = await _roomService.GetRoomAsync(gameId);
            room.State = RoomState.InGame;
            await _roomService.UpdateRoom(room);
        }

        public async Task NotifyOpponentImAlreadyInRoom(string playerId, string password)
        {
            int gameId = (int)Context.Items["gameId"];
            if (!await _gameService.ValidatePlayer(gameId, playerId, password, this))
            {
                return;
            }
            await Clients.Group(gameId.ToString()).SendAsync("AllPlayersJoinedToRoom");
        }

        public async Task MakeMove(int i, int j, string playerId, string password)
        {
            int gameId = (int)Context.Items["gameId"];
            if (!await _gameService.ValidatePlayer(gameId, playerId, password, this))
            {
                return;
            }
            await Clients
                .OthersInGroup(gameId.ToString())
                .SendAsync("OpponentMadeMove", i, j);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!Context.Items.ContainsKey("gameId")) return;
            int roomId = (int)Context.Items["gameId"];

            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            await Clients.Group(roomId.ToString()).SendAsync("OpponentDisconnected");
            await _roomService.DestroyRoomAsync(room.Id);
        }
    }
}

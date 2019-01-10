using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Database;
using TicTacToeServer.Enums;
using TicTacToeServer.Services;
using Microsoft.EntityFrameworkCore;

namespace TicTacToeServer.Hubs
{
    public class GameHub : Hub
    {
        const string _roomIdKey = "RoomId";

        readonly IGameService _gameService;
        readonly IRoomService _roomService;
        readonly Db _db;

        public GameHub(IGameService gameService, IRoomService roomService, Db db)
        {
           _gameService = gameService;
           _roomService = roomService;
            _db = db;
        }

        public async Task JoinToGame(int roomId, string playerId, string password)
        {
            try
            {
                var room = await _roomService.GetRoomAsync(roomId);
                if (!_gameService.ValidatePlayer(room, playerId, password))
                {
                    await _notifyCallerAccesDenied();
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
                Context.Items.Add(_roomIdKey, roomId);
                await Clients.OthersInGroup(roomId.ToString()).SendAsync("OpponentJoinedToGame");

                room.InitGame();
                room.State = RoomState.WaitingForSecondPlayer;
                room.Game.CurrentPlayerId = room.HostId;
                await _roomService.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                //log
            }
            
        }

        public async Task NotifyOpponentImAlreadyInRoom(string playerId, string password)
        {
            int roomId = (int)Context.Items[_roomIdKey];
            var room = await _roomService.GetRoomAsync(roomId);
            if (!_gameService.ValidatePlayer(room, playerId, password))
            {
                await _notifyCallerAccesDenied();
                return;
            }

            await Clients.Group(roomId.ToString()).SendAsync("AllPlayersJoinedToRoom");
            room.State = RoomState.InGame;
            await _roomService.SaveChangesAsync();
        }

        public async Task MakeMove(int i, int j, string playerId, string password)
        {
            int roomId = (int)Context.Items[_roomIdKey];
            var room = await _roomService.GetRoomWithGameAndGameField(roomId);
            if (!_gameService.ValidatePlayer(room, playerId, password))
            {
                await _notifyCallerAccesDenied();
                return;
            }

            if (!_gameService.IsPlayerTurn(room.Game, playerId))
            {
                await Clients.Caller.SendAsync("NotYourTurn", "It`s not your turn now!");
                return;
            }

            GameFieldFields simpleField = _gameService.Map2DimensionalParamsToGameSimpleField(i, j);

            if (!_gameService.CanMakeMove(room.Game.Field, simpleField))
            {
                await Clients.Caller.SendAsync("FieldAlreadyOccupied", "This field is not empty");
                return;
            }

            _gameService.MakeMove(room.Game, simpleField);

            if (room.Game.Field.IsFull)
            {
                if (_gameService.IsWinner(room.Game.Field))
                {
                    await Clients.Caller.SendAsync("Win");
                    await Clients.OthersInGroup(roomId.ToString()).SendAsync("Lose");
                }
                else
                {
                    await Clients.Group(roomId.ToString()).SendAsync("Draw");
                }
            }
            else if(_gameService.IsWinner(room.Game.Field))
            {
                await Clients.Caller.SendAsync("Win");
                await Clients.OthersInGroup(roomId.ToString()).SendAsync("Lose");
            }

            _gameService.NextPlayerTurn(room);
            await _gameService.SaveChangesAsync();

            await Clients
                .Group(roomId.ToString())
                .SendAsync("PlayerMadeMove", i, j);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!Context.Items.ContainsKey(_roomIdKey)) return;
            int roomId = (int)Context.Items[_roomIdKey];
            await Clients.Group(roomId.ToString()).SendAsync("OpponentDisconnected");

            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }
            _db.Remove(room);
            await _db.SaveChangesAsync();
        }

        private async Task _notifyCallerAccesDenied()
        {
            await Clients.Caller.SendAsync("AccedDenied");
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Database;
using TicTacToeServer.Enums;
using TicTacToeServer.Services;
using Microsoft.EntityFrameworkCore;
using TicTacToeServer.Hubs.Interfaces;

namespace TicTacToeServer.Hubs
{
    public class GameHub : Hub<IGameHubResponses>
    {
        public const string RoomIdKey = "RoomId";

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
            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null)
            {
                await Clients.Caller.RoomNotExist();
                return;
            }
            
            if (!_gameService.ValidatePlayer(room, playerId, password))
            {
                await _notifyCallerAccesDenied();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            Context.Items.Add(RoomIdKey, roomId);
            await Clients.OthersInGroup(roomId.ToString()).OpponentJoinedToGame();

            room.InitGame();
            room.State = RoomState.WaitingForSecondPlayer;
            room.Game.CurrentPlayerId = room.HostId;
            await _roomService.SaveChangesAsync();
        }

        public async Task NotifyOpponentImAlreadyInRoom(string playerId, string password)
        {
            int roomId = (int)Context.Items[RoomIdKey];
            var room = await _roomService.GetRoomAsync(roomId);
            if (!_gameService.ValidatePlayer(room, playerId, password))
            {
                await _notifyCallerAccesDenied();
                return;
            }

            await Clients.Group(roomId.ToString()).AllPlayersJoinedToRoom();
            room.State = RoomState.InGame;
            await _roomService.SaveChangesAsync();
        }

        public async Task MakeMove(int i, int j, string playerId, string password)
        {
            int roomId = (int)Context.Items[RoomIdKey];
            var room = await _roomService.GetRoomWithGameAndGameField(roomId);
            if (!_gameService.ValidatePlayer(room, playerId, password))
            {
                await _notifyCallerAccesDenied();
                return;
            }

            if (!_gameService.IsPlayerTurn(room.Game, playerId))
            {
                await Clients.Caller.NotYourTurn("It`s not your turn now!");
                return;
            }

            GameFieldFields simpleField = _gameService.Map2DimensionalParamsToGameSimpleField(i, j);

            if (!_gameService.CanMakeMove(room.Game.Field, simpleField))
            {
                await Clients.Caller.FieldAlreadyOccupied("This field is not empty");
                return;
            }

            _gameService.MakeMove(room.Game, simpleField);

            if (room.Game.Field.IsFull)
            {
                if (_gameService.IsWinner(room.Game.Field))
                {
                    await Clients.Caller.Win();
                    await Clients.OthersInGroup(roomId.ToString()).Lose();
                }
                else
                {
                    await Clients.Group(roomId.ToString()).Draw();
                }
            }
            else if(_gameService.IsWinner(room.Game.Field))
            {
                await Clients.Caller.Win();
                await Clients.OthersInGroup(roomId.ToString()).Lose();
            }

            _gameService.NextPlayerTurn(room);
            await _gameService.SaveChangesAsync();

            await Clients
                .Group(roomId.ToString())
                .PlayerMadeMove(i, j);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!Context.Items.ContainsKey(RoomIdKey)) return;
            int roomId = (int)Context.Items[RoomIdKey];
            await Clients.Group(roomId.ToString()).OpponentDisconnected();

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
            await Clients.Caller.AccesDenied();
        }
    }
}

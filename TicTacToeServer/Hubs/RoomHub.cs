using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicTacToeServer.Database;
using TicTacToeServer.Enums;
using TicTacToeServer.Models;
using TicTacToeServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TicTacToeServer.Extensions;
using TicTacToeServer.Hubs.Interfaces;

namespace TicTacToeServer.Hubs
{
    public class RoomHub : Hub<IRoomHubResponses>
    {
        //We need use instance of Db here directly, because in OnDisconnectedAsync
        //Db instance from services not exist
        readonly Db _db;
        public const string roomIdKey = "RoomId";

        readonly IRoomService _roomService;
        readonly object _numberOfPlayersInRoomLock = new object();

        public RoomHub(IRoomService roomService, Db db)
        {
            _roomService = roomService;
            _db = db;
        }

        [Authorize]
        public async Task CreateHostRoom(Room room)
        {
            _roomService.AddRoomWithHostInsideWithInLobbyState(room);
            await _roomService.SaveChangesAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            this.AddOrUpdateItemInContextItems(roomIdKey, room.Id);
            await Clients.Caller.HostRoomCreated(room.Id);
        }

        public async Task AddGuestToRoom(int roomId, string password, string guestNick)
        {
            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null) {
                await Clients.Caller.PlayerCannotJoinToRoom("Room not exist");
                return;
            }

            if ((password != null || password.Length > 0)
                && room.Password != password) {
                await Clients.Caller.PlayerCannotJoinToRoom("Wrong password");
                return;
            }

            string hostGuid;
            string guestGuid;
            lock (_numberOfPlayersInRoomLock)
            {
                room = _roomService.GetRoom(roomId);
                if (room == null || room.NumberOfPlayersInside == 2)
                {
                    Clients.Caller.PlayerCannotJoinToRoom("Room is full or not exist");
                    return;
                }

                room.NumberOfPlayersInside++;
                hostGuid = Guid.NewGuid().ToString();
                guestGuid = Guid.NewGuid().ToString();
                room.GuestNick = guestNick;
                room.HostId = hostGuid;
                room.GuestId = guestGuid;
                room.State = RoomState.WaitingForFirstPlayer;
                _roomService.SaveChanges();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            await Clients.OthersInGroup(roomId.ToString()).GuestJoinToRoom(roomId, hostGuid);
            await Clients.Caller.GuestJoinToRoom(roomId, guestGuid);
        }

        public async Task AbortRoom()
        {
            if (!Context.Items.ContainsKey(roomIdKey)) return;
            var roomId = (int)Context.Items[roomIdKey];
            
            _roomService.AttachAndDestroyRoom(roomId);
            await _roomService.SaveChangesAsync();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
            await Clients.Caller.RoomAborted();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!Context.Items.ContainsKey(roomIdKey)) return;
            int roomId = (int)Context.Items[roomIdKey];

            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            if (room.State == RoomState.InLobby)
            {
                _db.Rooms.Remove(room);
                await _db.SaveChangesAsync();
            }  
        }
    }
}

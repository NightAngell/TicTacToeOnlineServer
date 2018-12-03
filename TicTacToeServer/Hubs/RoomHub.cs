using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicTacToeServer.Enums;
using TicTacToeServer.Models;
using TicTacToeServer.Services.Interfaces;

namespace TicTacToeServer.Hubs
{
    public class RoomHub : Hub
    {
        readonly IRoomService _roomService;
        readonly object _roomLock = new object();

        public RoomHub(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task CreateHostRoom(Room room)
        {
            room.NumberOfPlayersInside = 1;
            room.State = RoomState.InLobby;
            await _roomService.AddRoomAsync(room);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"RoomId{room.Id}");
            _attachRoomIdToConnectionContext(room.Id);
            await Clients.Caller.SendAsync("HostRoomCreated", room.Id);
        }

        public async Task AddGuestToRoom(int roomId, string password, string guestNick)
        {
            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null) {
                await Clients.Caller.SendAsync("PlayerCannotJoinToRoom", "Room not exist");
                return;
            }

            if ((password != null || password.Length > 0)
                && room.Password != password) {
                await Clients.Caller.SendAsync("PlayerCannotJoinToRoom", "Wrong password");
                return;
            }

            lock (_roomLock)
            {
                room = _roomService.GetRoom(roomId);
                if (room == null || room.NumberOfPlayersInside == 2)
                {
                    Clients.Caller.SendAsync("PlayerCannotJoinToRoom", "Room is full or not exist");
                    return;
                }
                room.NumberOfPlayersInside++;
            }
            
            string hostGuid = Guid.NewGuid().ToString();
            string guestGuid = Guid.NewGuid().ToString();

            room.GuestNick = guestNick;
            room.HostId = hostGuid;
            room.GuestId = guestGuid;
            room.State = RoomState.ReadyForGame;
            await _roomService.UpdateRoom(room);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"RoomId{roomId}");
            await Clients.OthersInGroup($"RoomId{roomId}").SendAsync("GuestJoinToRoom", roomId, hostGuid);
            await Clients.Caller.SendAsync("GuestJoinToRoom", roomId, guestGuid);
        }

        public async Task AbortRoom()
        {
            if (!Context.Items.ContainsKey("RoomId")) return;
            var roomId = (int)Context.Items["RoomId"];
            
            await _roomService.DestroyRoomAsync(roomId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"RoomId{roomId}");
            await Clients.Caller.SendAsync("RoomAborted");
        }


        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!Context.Items.ContainsKey("RoomId")) return;
            int roomId = (int)Context.Items["RoomId"];

            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            if(room.State == RoomState.InLobby)
            {
                await _roomService.DestroyRoomAsync(room.Id);
            }  
        }

        private void _attachRoomIdToConnectionContext(int roomID)
        {
            if (Context.Items.ContainsKey("RoomId"))
            {
                Context.Items["RoomId"] = roomID;
            }else
            {
                Context.Items.Add("RoomId", roomID);
            }
        }
    }
}

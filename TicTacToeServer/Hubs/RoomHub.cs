using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Models;
using TicTacToeServer.Services.Interfaces;

namespace TicTacToeServer.Hubs
{
    public class RoomHub : Hub
    {
        readonly IRoomService _roomService;

        public RoomHub(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task CreateHostRoom(Room room)
        {
            await _roomService.AddRoomAsync(room);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"RoomId{room.Id}");
            _attachRoomIdToConnectionContext(room.Id);
            await Clients.Caller.SendAsync("HostRoomCreated", room.Id);
        }

        public async Task AddGuestToRoom(int roomId, string password)
        {
            if (!await _roomService.RoomExist(roomId)) {
                await Clients.Caller.SendAsync("PlayerCannotJoinToRoom", "Room not exist");
                return;
            }

            if ((password != null || password.Length > 0)
                && !_roomService.IsPasswordGood(roomId, password)) {
                await Clients.Caller.SendAsync("PlayerCannotJoinToRoom", "Wrong password");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"RoomId{roomId}");
            await Clients.Group($"RoomId{roomId}").SendAsync("GuestJoinToRoom", roomId);
            await _roomService.DestroyRoom(roomId);
        }

        public async Task AbortRoom()
        {
            if (!Context.Items.ContainsKey("RoomId")) return;
            var roomId = (int)Context.Items["RoomId"];
            
            await _roomService.DestroyRoom(roomId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"RoomId{roomId}");
            await Clients.Caller.SendAsync("RoomAborted");
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!Context.Items.ContainsKey("RoomId")) return;
            var roomId = (int)Context.Items["RoomId"];

            await _roomService.DestroyRoom(roomId);

            await base.OnDisconnectedAsync(exception);
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

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
            await Clients.Caller.SendAsync("HostRoomCreated", room.Id);
        }

        public async Task AddGuestToRoom(int roomId)
        {
            if (!await _roomService.RoomExist(roomId)) {
                await Clients.Caller.SendAsync("PlayerCannotJoinToRoom", "Room not exist");
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, $"RoomId{roomId}");
            await Clients.Group($"RoomId{roomId}").SendAsync("GuestJoinToRoom", roomId);
            await _roomService.DestroyRoom(roomId);
        }

        public async Task AbortRoom(int roomId)
        {
            await _roomService.DestroyRoom(roomId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"RoomId{roomId}");
            await Clients.Caller.SendAsync("RoomAborted");
        }
    }
}

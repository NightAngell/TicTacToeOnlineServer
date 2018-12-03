using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Services.Interfaces;

namespace TicTacToeServer.Services
{
    public class GameService : IGameService
    {
        readonly IRoomService roomService;

        public GameService(IRoomService roomService)
        {
            this.roomService = roomService;
        }

        public async Task<bool> ValidatePlayer(int gameId, string playerId, string password, Hub gameHub)
        {
            bool isValid = true;
            var room = await roomService.GetRoomAsync(gameId);
            if (room == null)
            {
                await gameHub.Clients.Caller.SendAsync("RoomNotExist");
                isValid = false;
            }
            if (
                !(
                    (room.HostId == playerId && room.Password == password)
                    || (room.GuestId == playerId && room.Password == password)
                )
              )
            {
                await gameHub.Clients.Caller.SendAsync("AccedDenied", "");
                isValid = false;
            }

            return isValid;
        }
    }
}

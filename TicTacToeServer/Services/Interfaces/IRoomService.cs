using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.DTO;
using TicTacToeServer.Models;

namespace TicTacToeServer.Services.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetListOfRoomsAsync();
        Task<IEnumerable<RoomDto>> GetListOfRoomsDtosInLobbyAsync();
        Task AddRoomAsync(Room room);
        Task DestroyRoomAsync(int roomId);
        Task<Room> GetRoomAsync(int roomId);
        Room GetRoom(int roomId);
        Task UpdateRoom(Room room);
    }
}

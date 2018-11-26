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
        Task<IEnumerable<RoomDto>> GetListOfRoomDtosAsync();
        Task AddRoomAsync(Room room);
        Task DestroyRoom(int roomId);
        Task<bool> RoomExist(int roomId);
        bool IsPasswordGood(int roomId, string password);
    }
}

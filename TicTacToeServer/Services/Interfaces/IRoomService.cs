using System.Collections.Generic;
using System.Threading.Tasks;
using TicTacToeServer.DTO;
using TicTacToeServer.Enums;
using TicTacToeServer.Models;

namespace TicTacToeServer.Services
{
    public interface IRoomService
    {
        void AddRoom(Room room);
        void AddRoomWithHostInsideWithInLobbyState(Room room);
        void AttachAndDestroyRoom(int roomId);
        void SaveChanges();
        Task<IEnumerable<Room>> GetListOfRoomsAsync();
        Task<IEnumerable<RoomDto>> GetListOfRoomsDtosInLobbyAsync();
        Room GetRoom(int roomId);
        Task<Room> GetRoomAsync(int roomId);
        Task SaveChangesAsync();
        Task SetStateAndSaveChangesAsync(int roomId, RoomState state);
        Task<Room> GetRoomWithGameAndGameField(int roomId);
    }
}
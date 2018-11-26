using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.DTO;
using TicTacToeServer.Models;
using TicTacToeServer.Services.Interfaces;

namespace TicTacToeServer.Services
{
    public class RoomService : IRoomService
    {
        static readonly List<Room> _rooms = new List<Room>();
        static int _roomId = 0;
        public async Task<IEnumerable<Room>> GetListOfRoomsAsync()
        {
            return await Task.FromResult(_rooms);
        }

        public async Task<IEnumerable<RoomDto>> GetListOfRoomDtosAsync()
        {
            var roomDtos = new List<RoomDto>();
            foreach (var room in _rooms)
            {
                var roomDto = new RoomDto();
                roomDto.Id = room.Id;
                roomDto.IsPassword = room.Password != null && room.Password.Length > 0;
                roomDto.HostNick = room.HostNick;
                roomDtos.Add(roomDto);
            }
            return roomDtos;
        }

        public async Task AddRoomAsync(Room room)
        {
            room.Id = _getPseudoUniqueId();
            _rooms.Add(room);
        }

        public async Task<bool> RoomExist(int roomId)
        {
            return await Task.FromResult(_rooms.FindIndex(r => r.Id == roomId) != -1);
        }

        public async Task DestroyRoom(int roomId)
        {
            _rooms.Remove(
                _rooms.Find(r => r.Id == roomId)
            );
        }

        public bool IsPasswordGood(int roomId, string password)
        {
            var room = _rooms.Find(r => r.Id == roomId);
            return room.Password == password;
        }

        /**
         * <summary>
         * This is enoguh for this app,
         * because we it`s not possible to play int.Max players simultaneously
         * </summary>
         */
        private int _getPseudoUniqueId()
        {
            _roomId++;
            if (_roomId < 0) _roomId = 1;
            return _roomId;
        }
    }
}

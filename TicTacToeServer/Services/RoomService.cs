using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.DTO;
using TicTacToeServer.Enums;
using TicTacToeServer.Models;
using TicTacToeServer.Services.Interfaces;

namespace TicTacToeServer.Services
{
    public class RoomService : IRoomService
    {
        static readonly List<Room> _rooms = new List<Room>();
        static int _roomId = 0;

        readonly object _newRoomIdLock = new object();
        public async Task<IEnumerable<Room>> GetListOfRoomsAsync()
        {
            return await Task.FromResult(_rooms);
        }

        public async Task<IEnumerable<RoomDto>> GetListOfRoomsDtosInLobbyAsync()
        {
            var roomDtos = new List<RoomDto>();
            foreach (var room in _rooms)
            {
                if (room.State != RoomState.InLobby) continue;
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
            lock (_newRoomIdLock)
            {
                room.Id = _getPseudoUniqueId();
                _rooms.Add(room);
            }
        }

        public async Task DestroyRoomAsync(int roomId)
        {
            _rooms.Remove(
                _rooms.Find(r => r.Id == roomId)
            );
        }

        public Task<Room> GetRoomAsync(int roomId)
        {
            return Task.FromResult(_rooms.Find(r => r.Id == roomId));
        }

        public Room GetRoom(int roomId)
        {
            return _rooms.Find(r => r.Id == roomId);
        }

        public async Task UpdateRoom(Room room)
        {
            int index = _rooms.FindIndex(r => r.Id == room.Id);
            _rooms[index] = room;
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

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Database;
using TicTacToeServer.DTO;
using TicTacToeServer.Enums;
using TicTacToeServer.Models;

namespace TicTacToeServer.Services
{
    public class RoomService : IRoomService
    {
        readonly Db _db;
        readonly IMapper _mapper;

        public RoomService(Db db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Room>> GetListOfRoomsAsync()
        {
            return await _db.Rooms.ToListAsync();
        }

        public async Task<IEnumerable<RoomDto>> GetListOfRoomsDtosInLobbyAsync()
        {
            var roomDtos = new List<RoomDto>();
            var rooms = await _db.Rooms.ToListAsync();
            foreach (var room in rooms)
            {
                if (room.State != RoomState.InLobby) continue;
                var roomDto = _mapper.Map<RoomDto>(room);
                roomDtos.Add(roomDto);
            }
            return roomDtos;
        }

        public void AddRoom(Room room)
        {
            _db.Rooms.Add(room);
        }

        public void AddRoomWithHostInsideWithInLobbyState(Room room)
        {
            room.NumberOfPlayersInside = 1;
            room.State = RoomState.InLobby;
            _db.Rooms.Add(room);
        }

        public void AttachAndDestroyRoom(int roomId)
        {
            var room = new Room()
            {
                Id = roomId
            };
            _db.Rooms.Attach(room);
            _db.Rooms.Remove(room);
        }

        public async Task<Room> GetRoomAsync(int roomId)
        {
            return await _db.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<Room> GetRoomWithGameAndGameField(int roomId)
        {
            return await _db.Rooms
                .Include(r => r.Game)
                .ThenInclude(g => g.Field) 
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public Room GetRoom(int roomId)
        {
            return _db.Rooms.FirstOrDefault(r => r.Id == roomId);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task SetStateAndSaveChangesAsync(int roomId, RoomState state)
        {
            var room = await GetRoomAsync(roomId);
            room.State = state;
            await SaveChangesAsync();
        }
    }
}

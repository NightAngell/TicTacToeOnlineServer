using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.DTO;
using TicTacToeServer.Models;

namespace TicTacToeServer.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Room, RoomDto>()
                 .ConstructUsing(x => new RoomDto(
                     x.Id,
                     x.Password != null && x.Password.Length > 0,
                     x.HostNick
                 )
            );
        }
    }
}

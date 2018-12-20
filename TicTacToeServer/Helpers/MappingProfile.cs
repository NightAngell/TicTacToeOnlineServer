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
                .ForMember(r => r.IsPassword, m => m.MapFrom(rd => rd.Password != null && rd.Password.Length > 0 ));
        }
    }
}

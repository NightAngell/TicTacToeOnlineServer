using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.DTO
{
    public class RoomDto
    {
        public RoomDto(int id, bool isPassword, string hostNick)
        {
            Id = id;
            IsPassword = isPassword;
            HostNick = hostNick;
        }

        public int Id { get; private set; }
        public bool IsPassword { get; private set; }
        public string HostNick { get; private set; }
    }
}

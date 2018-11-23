using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.DTO
{
    public class RoomDto
    {
        public int Id { get; set; }
        public bool IsPassword { get; set; }
        public string HostNick { get; set; }
    }
}

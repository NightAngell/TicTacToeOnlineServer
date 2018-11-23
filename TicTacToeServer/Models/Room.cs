using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string HostNick { get; set; }
    }
}

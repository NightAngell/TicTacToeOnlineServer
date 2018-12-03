using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Enums;

namespace TicTacToeServer.Models
{
    public class Room
    {
        RoomState _roomState;

        public int Id { get; set; }
        public string Password { get; set; }
        public string HostNick { get; set; }
        public string HostId { get; set; }
        public string GuestNick { get; set; }
        public string GuestId { get; set; }
        public int NumberOfPlayersInside { get; set; }
        public RoomState State
        {
            set
            {
                LastStateChangeDate = DateTime.UtcNow;
                _roomState = value;
            }
            get => _roomState;
        }
        public DateTime LastStateChangeDate { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.Models
{
    public class Game
    {
        public int Id { get; set; }

        public int FieldId { get; set; }
        [Required]
        public GameField Field { get; set; }

        public string CurrentPlayerId { get; set; }

        public int RoomId { get; set; }
        [Required]
        [ForeignKey("RoomId")]
        public Room Room { get; set; }
    }
}

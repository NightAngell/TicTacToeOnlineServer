using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.Models
{
    public class GameField
    {
        public int Id { get; set; }

        public string TopLeft { get; set; }
        public string Top { get; set; }
        public string TopRight { get; set; }

        public string MiddleLeft { get; set; }
        public string Middle { get; set; }
        public string MiddleRight { get; set; }

        public string DownLeft { get; set; }
        public string Down { get; set; }
        public string DownRight { get; set; }

        public int GameId { get; set; }
        [Required]
        [ForeignKey("GameId")]
        public Game Game { get; set; }

        [NotMapped]
        public bool IsFull
        {
            get
            {
                return !string.IsNullOrEmpty(TopLeft)
                 && !string.IsNullOrEmpty(Top)
                 && !string.IsNullOrEmpty(TopRight)
                 && !string.IsNullOrEmpty(MiddleLeft)
                 && !string.IsNullOrEmpty(Middle)
                 && !string.IsNullOrEmpty(MiddleRight)
                 && !string.IsNullOrEmpty(DownLeft)
                 && !string.IsNullOrEmpty(Down)
                 && !string.IsNullOrEmpty(DownRight);
            }
        }
    }
}

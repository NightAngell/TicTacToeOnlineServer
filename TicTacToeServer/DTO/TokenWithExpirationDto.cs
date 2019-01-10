using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.DTO
{
    public class TokenWithExpirationDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}

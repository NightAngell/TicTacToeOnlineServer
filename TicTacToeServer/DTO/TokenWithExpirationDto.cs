using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.DTO
{
    public class TokenWithExpirationDto
    {
        public TokenWithExpirationDto(string token, DateTime validTo)
        {
            Token = token;
            Expiration = validTo;
        }

        public string Token { get; private set; }
        public DateTime Expiration { get; private set; }
    }
}

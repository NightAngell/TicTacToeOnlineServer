using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.DTO
{
    public class LoginDto
    {
        public LoginDto(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [Required]
        [EmailAddress]
        public string Email { get; private set; }

        [Required]
        public string Password { get; private set; }
    }
}

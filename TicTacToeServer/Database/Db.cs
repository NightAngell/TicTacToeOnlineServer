using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Models;

namespace TicTacToeServer.Database
{
    public class Db : DbContext
    {
        public Db(DbContextOptions<Db> options): base(options){ }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<GameField> GameField { get; set; }
    }
}

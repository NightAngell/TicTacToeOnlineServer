using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Models;

namespace TicTacToeServer.Database
{
    public class Db : IdentityDbContext<AppUser>
    {
        public Db(DbContextOptions<Db> options): base(options){ }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<GameField> GameField { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", Name = "User", NormalizedName = "USER"},
                new { Id = "2", Name = "Admin", NormalizedName = "ADMIN" },
                new { Id = "3", Name = "Banned", NormalizedName = "BANNED" }
            );
        }
    }
}

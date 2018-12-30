using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TestSupport.EfHelpers;
using TicTacToeServer.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TicTacToeServer.Models;

namespace TicTacToeServerTests.Services
{
    /// <summary>
    /// https://github.com/JonPSmith/EfCore.TestSupport/wiki/1.-Sqlite-in-memory-test-database
    /// </summary>
    [TestFixture]
    class RoomServiceTests
    {
        [Test]
        public void Test()
        {
            using (var context = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                context.Database.EnsureCreated();
                context.Rooms.Add(new Room() { HostNick="hn"});
                context.SaveChanges();
                Assert.IsTrue(context.Rooms.Count() == 1);
            }
        }
    }
}

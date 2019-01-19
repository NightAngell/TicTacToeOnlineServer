using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeServer.Models;

namespace TicTacToeServerTests.Models
{
    [TestFixture]
    class RoomTests
    {
        [Test]
        public void InitRoom_RoomInitWithGameAndGameField()
        {
            var room = new Room();
            room.InitGame();

            Assert.NotNull(room.Game);
            Assert.NotNull(room.Game.Field);
        }
    }
}

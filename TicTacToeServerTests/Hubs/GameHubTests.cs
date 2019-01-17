using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicTacToeServer.Database;
using TicTacToeServer.Hubs;
using TicTacToeServer.Services;

namespace TicTacToeServerTests.Hubs
{
    [TestFixture]
    class GameHubTests
    {
        GameHub _gameHub;
        Mock<IGameService> _gameServiceMock;
        Mock<IRoomService> _roomServiceMock;
        Mock<Db> _dbMock;

        [SetUp]
        public void SetUp()
        {
            _gameServiceMock = new Mock<IGameService>();
            _roomServiceMock = new Mock<IRoomService>();
            _dbMock = new Mock<Db>();
        }

        //JoinToGame
        [Test]
        public async Task JoinToGame_RoomNotExist_Null()
        {
            _gameHub.JoinToGame(1, "3", "password");
        }

        private void  _initGameHub(Db db)
        {
            _gameHub = new GameHub(_gameServiceMock.Object, _roomServiceMock.Object, db);
        }
    }
}

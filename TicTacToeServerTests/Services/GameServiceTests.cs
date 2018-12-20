using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeServer.Models;
using TicTacToeServer.Services;

namespace TicTacToeServerTests.Services
{
    [TestFixture]
    class GameServiceTests
    {
        Mock<IRoomService> _roomServiceMock;
        GameService _gameService;

        readonly private string _nv = "notValid";
        private Room _getRoomWithValidData()
        {
            return new Room
            {
                HostId = "abc",
                GuestId = "bca",
                Password = "test",
            };
        }

        [SetUp]
        public void SetUp()
        {
            _roomServiceMock = new Mock<IRoomService>();

            _gameService = new GameService(_roomServiceMock.Object);
        }

        [Test]
        public void ValidatePlayer_PlayersWithValidDataExist_ReturnsTrue()
        {
            var room = _getRoomWithValidData();

            Assert.IsTrue(_gameService.ValidatePlayer(room, room.HostId, room.Password));
            Assert.IsTrue(_gameService.ValidatePlayer(room, room.GuestId, room.Password));
        }

        [Test]
        public void ValidatePlayer_PlayerHaveNotValidId_ReturnsFalse()
        {
            var room = _getRoomWithValidData();

            Assert.IsFalse(
                _gameService.ValidatePlayer(room, $"{room.HostId}{_nv}", room.Password)
            );
        }

        [Test]
        public void ValidatePlayer_GuestHaveNotValidPassword_ReturnsFalse()
        {
            var room = _getRoomWithValidData();

            Assert.IsFalse(
                _gameService.ValidatePlayer(room, room.GuestId, $"{room.Password}{_nv}")
            );
        }

        [Test]
        public void ValidatePlayer_HostHaveNotValidPassword_ReturnsFalse()
        {
            var room = _getRoomWithValidData();

            Assert.IsFalse(
                _gameService.ValidatePlayer(room, room.HostId, $"{room.Password}{_nv}")
            );
        }

        [Test]
        public void ValidatePlayer_AllPlayersDatasNotValid_ReturnsFalse()
        {
            var room = _getRoomWithValidData();

            Assert.IsFalse(
                _gameService.ValidatePlayer(
                    room,
                    $"{room.HostId}{_nv}",
                    $"{room.Password}{_nv}"
                )
            );
            Assert.IsFalse(
               _gameService.ValidatePlayer(
                   room,
                   $"{room.GuestId}{_nv}",
                   $"{room.Password}{_nv}"
               )
           );
        }


    }
}

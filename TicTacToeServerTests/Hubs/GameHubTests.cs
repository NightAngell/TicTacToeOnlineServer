using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicTacToeServer.Database;
using TicTacToeServer.Enums;
using TicTacToeServer.Hubs;
using TicTacToeServer.Hubs.Interfaces;
using TicTacToeServer.Models;
using TicTacToeServer.Services;

namespace TicTacToeServerTests.Hubs
{
    [TestFixture]
    class GameHubTests : HubTestsBase<IGameHubResponses, Db>
    {
        GameHub _gameHub;
        Mock<IGameService> _gameServiceMock;
        Mock<IRoomService> _roomServiceMock;
        const string ValidPassword = "ValidPassword";
        const string InvalidPassword = "InvalidPassword";
        const string ValidPlayerNick = "PlayerNick";
        const string ValidPlayerId = "PlayerId";
        const string InvalidPlayerId = "Inv";
        const int RoomNotExistId = 1;
        const int RoomExistId = 5;
        Room _existingRoom;

        [SetUp]
        public void SetUp()
        {
            BaseSetUp();
            _gameServiceMock = new Mock<IGameService>();
            _roomServiceMock = new Mock<IRoomService>();
            _existingRoom = new Room() {
                Id = RoomExistId,
                HostNick = ValidPlayerNick,
                Password = ValidPassword,
                HostId = ValidPlayerId
            };
            _roomServiceMock
                .Setup(x => x.GetRoomAsync(RoomExistId))
                .ReturnsAsync(_existingRoom);
            _gameServiceMock
                .Setup(x => x.ValidatePlayer(It.IsAny<Room>(), InvalidPlayerId, ValidPassword))
                .Returns(false);
            _gameServiceMock
                .Setup(x => x.ValidatePlayer(It.IsAny<Room>(), ValidPlayerId, InvalidPassword))
                .Returns(false);
            _gameServiceMock
                .Setup(x => x.ValidatePlayer(It.IsAny<Room>(), ValidPlayerId, ValidPassword))
                .Returns(true);
        }

        //JoinToGame
        [Test]
        public async Task JoinToGame_RoomNotExist_ClientNotifiedRoomNotExist()
        {
            _initGameHub(_dbMock.Object);
            await _gameHub.JoinToGame(RoomNotExistId, ValidPlayerId, ValidPassword);
           
            _clientMock.Verify(x => x.RoomNotExist(), Times.Once);
        }

        [Test]
        public async Task JoinToGame_RoomNotExist_OthersInGroupNeverNotifiedOpponentJoinedToGame()
        {
            _initGameHub(_dbMock.Object);
            await _gameHub.JoinToGame(RoomNotExistId, ValidPlayerId, ValidPassword);
            _responsesMock.Verify(x => x.OpponentJoinedToGame(), Times.Never);
        }

        [Test]
        public async Task JoinToGame_InvalidPlayerIdOrPassword_ClientNotifiedAccesDenied()
        {
            _initGameHub(_dbMock.Object);
            await _gameHub.JoinToGame(RoomExistId, InvalidPlayerId, ValidPassword);
            _clientMock.Verify(x => x.AccesDenied(), Times.Once);
            await _gameHub.JoinToGame(RoomExistId, ValidPlayerId, InvalidPassword);
            _clientMock.Verify(x => x.AccesDenied(), Times.Exactly(2));
        }

        [Test]
        public async Task JoinToRoom_InvalidPlayerIdOrPassword_OthersInGroupNeverNotifiedOpponentJoinedToGame()
        {
            _initGameHub(_dbMock.Object);
            await _gameHub.JoinToGame(RoomExistId, InvalidPlayerId, ValidPassword);
            _clientMock.Verify(x => x.OpponentJoinedToGame(), Times.Never);
            await _gameHub.JoinToGame(RoomExistId, ValidPlayerId, InvalidPassword);
            _clientMock.Verify(x => x.AccesDenied(), Times.Exactly(2));
        }

        [Test]
        public async Task JoinToRoom_AllDataValid_OthersInGroupNotifiedOpponentJoinedToGame()
        {
            await _initGameHubAndCallJoinToGameWithAllParamsValid();

            _responsesMock.Verify(
                x => x.OpponentJoinedToGame(),
                Times.Once
            );
        }

        [Test]
        public async Task JoinToRoom_AllDataValid__PlayerAddedToGroupAssociatedWithRoom()
        {
            await _initGameHubAndCallJoinToGameWithAllParamsValid();

            _verifySomebodyAddedToGroup(Times.Once(), _existingRoom.Id.ToString());
            _verifySomebodyAddedToGroup(Times.Once());
        }

        [Test]
        public async Task JoinToRoom_AllDataValid_RoomIdAddedToContextItems()
        {
            await _initGameHubAndCallJoinToGameWithAllParamsValid();

            _verifyContextItemsContainKeyValuePair(GameHub.RoomIdKey, _existingRoom.Id);
        }

        [Test]
        public async Task JoinToRoom_AllDataValid_RoomStateSaved()
        {
            await _initGameHubAndCallJoinToGameWithAllParamsValid();

            _roomServiceMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task JoinToRoom_AllDataValid_RoomGetWaitingForSecondPlayerState()
        {
            await _initGameHubAndCallJoinToGameWithAllParamsValid();

            Assert.True(_existingRoom.State == RoomState.WaitingForSecondPlayer);
        }

        [Test]
        public async Task JoinToRoom_AllDataValid_RoomGetGameCurrentPlayerId()
        {
            await _initGameHubAndCallJoinToGameWithAllParamsValid();

            Assert.True(_existingRoom.Game.CurrentPlayerId.Length > 0);
        }

        private async Task _initGameHubAndCallJoinToGameWithAllParamsValid()
        {
            _initGameHub(_dbMock.Object);
            await _gameHub.JoinToGame(RoomExistId, ValidPlayerId, ValidPassword);
        }

        //NotifyOpponentImAlreadyInRoom
        [Test]
        public async Task NotifyOpponentImAlreadyInRoom_InvalidPlayerIdOrPassword_CallerNotifiedAccesDenied()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.NotifyOpponentImAlreadyInRoom(InvalidPlayerId, InvalidPassword);

            _clientMock.Verify(x => x.AccesDenied(), Times.Once);
        }

        [Test]
        public async Task NotifyOpponentImAlreadyInRoom_InvalidPlayerIdOrPassword_GroupNeverNotifiedAllPlayersJoinedToRoom()
        {
            _addRoomIdToContextItemsAndInitGameHub();

             await _gameHub.NotifyOpponentImAlreadyInRoom(InvalidPlayerId, InvalidPassword);

            _responsesMock.Verify(x => x.AllPlayersJoinedToRoom(), Times.Never);
        }

        [Test]
        public async Task NotifyOpponentImAlreadyInRoom_ValidLoginAndPassword_GroupNotifiedAllPlayersJoinedToRoom()
        {
            _addRoomIdToContextItemsAndInitGameHub();

             await _gameHub.NotifyOpponentImAlreadyInRoom(ValidPlayerId, ValidPassword);

            _responsesMock.Verify(x => x.AllPlayersJoinedToRoom(), Times.Once);
        }

        [Test]
        public async Task NotifyOpponentImAlreadyInRoom_ValidLoginAndPassword_RoomChangeStateToInGameAndItsSaved()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.NotifyOpponentImAlreadyInRoom(ValidPlayerId, ValidPassword);

            Assert.AreEqual(_existingRoom.State, RoomState.InGame);
            _roomServiceMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        private void _addRoomIdToContextItemsAndInitGameHub()
        {
            _itemsFake.Add(GameHub.RoomIdKey, _existingRoom.Id);
            _initGameHub(_dbMock.Object);
        }

        //MakeMove
        //[Test]
        //public async MakeMove_

        private void  _initGameHub(Db db)
        {
            _gameHub = new GameHub(_gameServiceMock.Object, _roomServiceMock.Object, db) {
                Context = _contextMock.Object,
                Groups = _groupsMock.Object,
                Clients = _clientsMock.Object
            };
        }
    }
}

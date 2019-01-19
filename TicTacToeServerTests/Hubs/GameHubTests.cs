using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestSupport.EfHelpers;
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
            _roomServiceMockSetups();
            _gameServiceMockSetups();
        }

        private void _roomServiceMockSetups()
        {
            _roomServiceMock
               .Setup(x => x.GetRoomAsync(RoomExistId))
               .ReturnsAsync(_existingRoom);
            _roomServiceMock
               .Setup(x => x.GetRoomWithGameAndGameField(RoomExistId))
               .Callback(_addToExistingRoomGameWithGameFieldAndCurrentPlayerId)
               .ReturnsAsync(_existingRoom);
        }

        private void _gameServiceMockSetups()
        {
            _gameServiceMock
                .Setup(x => x.ValidatePlayer(It.IsAny<Room>(), InvalidPlayerId, ValidPassword))
                .Returns(false);
            _gameServiceMock
                .Setup(x => x.ValidatePlayer(It.IsAny<Room>(), ValidPlayerId, InvalidPassword))
                .Returns(false);
            _gameServiceMock
               .Setup(x => x.ValidatePlayer(It.IsAny<Room>(), InvalidPlayerId, InvalidPassword))
               .Returns(false);
            _gameServiceMock
                .Setup(x => x.ValidatePlayer(It.IsAny<Room>(), ValidPlayerId, ValidPassword))
                .Returns(true);
            _gameServiceMock
                .Setup(x => x.IsPlayerTurn(It.IsAny<Game>(), ValidPlayerId))
                .Returns(true);
            _gameServiceMock
               .Setup(x => x.CanMakeMove(It.IsAny<GameField>(), It.IsAny<GameFieldFields>()))
               .Returns(true);
        }

        private void _addToExistingRoomGameWithGameFieldAndCurrentPlayerId()
        {
            _existingRoom.Game = new Game {
                CurrentPlayerId = ValidPlayerId,
                Field = new GameField()
            };
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
        [Test]
        public async Task MakeMove_InvalidLogin_CallerGetAccesDenied()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, InvalidPlayerId, ValidPassword);

            _clientMock.Verify(x => x.AccesDenied(), Times.Once);
        }

        [Test]
        public async Task MakeMove_InvalidPassword_CallerGetAccesDenied()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, InvalidPassword);

            _clientMock.Verify(x => x.AccesDenied(), Times.Once);
        }

        [Test]
        public async Task MakeMove_InvalidPasswordAndLogin_CallerGetAccesDenied()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, InvalidPassword);

            _clientMock.Verify(x => x.AccesDenied(), Times.Once);
        }

        [Test]
        public async Task MakeMove_InvalidCredintials_GroupNeverGetPlayerMadeMove()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, InvalidPassword);
            _verifyGroupNeverGetPlayerMadeMove();
             await _gameHub.MakeMove(1, 1, InvalidPlayerId, ValidPassword);
            _verifyGroupNeverGetPlayerMadeMove();
            await _gameHub.MakeMove(1, 1, InvalidPlayerId, InvalidPassword);
            _verifyGroupNeverGetPlayerMadeMove();
        }

        [Test]
        public async Task MakeMove_IsNotPlayerTurn_CallerGetNotYourTurnNotification()
        {
            await _makeMove_IsNotPlayerTurnScenario();

            _clientMock.Verify(x => x.NotYourTurn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task MakeMove_IsNotPlayerTurn_GropuNeverGetPlayerMadeMove()
        {
            await _makeMove_IsNotPlayerTurnScenario();

            _verifyGroupNeverGetPlayerMadeMove();
        }

        private async Task _makeMove_IsNotPlayerTurnScenario()
        {
            _gameServiceMock
               .Setup(x => x.IsPlayerTurn(It.IsAny<Game>(), It.IsAny<string>()))
               .Returns(false);
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);
        }

        [Test]
        public async Task MakeMove_FieldIsNotEmpty_CallerGetFieldAlreadyOccupied()
        {
            await _makeMove_FieldIsNotEmptyScenario();

            _clientMock.Verify(x => x.FieldAlreadyOccupied(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task MakeMove_FieldIsNotEmpty_GropuNeverGetPlayerMadeMove()
        {
            await _makeMove_FieldIsNotEmptyScenario();

            _clientMock.Verify(x => x.FieldAlreadyOccupied(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task MakeMove_RoomIsNotFullAndThereIsNoWinner_GroupNotifiedPlayerMadeMove()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);

            _responsesMock.Verify(
                x => x.PlayerMadeMove(It.IsAny<int>(), It.IsAny<int>()),
                Times.Once
            );
        }

        [Test]
        public async Task MakeMove_RoomIsNotFullAndThereIsNoWinner_NextPlayerIsSet()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);

            _gameServiceMock.Verify(x => x.NextPlayerTurn(It.IsAny<Room>()), Times.Once);
        }

        [Test]
        public async Task MakeMove_RoomIsNotFullButIsWinner_CallerNotifyWinOthersInGroupNotifyLose()
        {
            _gameServiceMock
                .Setup(x => x.IsWinner(It.IsAny<GameField>()))
                .Returns(true);
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);

            _clientMock.Verify(x => x.Win(), Times.Once);
            _responsesMock.Verify(x => x.Lose(), Times.Once);
        }

        [Test]
        public async Task MakeMove_IsDraw_GroupNotifiedDraw()
        {
            _gameServiceMock
                .Setup(x => x.IsWinner(It.IsAny<GameField>()))
                .Returns(false);
            _roomServiceMock
              .Setup(x => x.GetRoomWithGameAndGameField(RoomExistId))
              .Callback(()=> {
                  _existingRoom.Game = new Game {
                      CurrentPlayerId = ValidPlayerId,
                      Field = _getFullField()
                  };
              })
              .ReturnsAsync(_existingRoom);
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);

            _responsesMock.Verify(x => x.Draw(), Times.Once);
        }

        [Test]
        public  async Task MakeMove_RoomIsFullAndIsWinner_CallerNotifyWinOthersInGroupNotifyLose()
        {
            _gameServiceMock
                .Setup(x => x.IsWinner(It.IsAny<GameField>()))
                .Returns(true);
            _roomServiceMock
              .Setup(x => x.GetRoomWithGameAndGameField(RoomExistId))
              .Callback(() => {
                  _existingRoom.Game = new Game
                  {
                      CurrentPlayerId = ValidPlayerId,
                      Field = _getFullField()
                  };
              })
              .ReturnsAsync(_existingRoom);
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);

            _clientMock.Verify(x => x.Win(), Times.Once);
            _responsesMock.Verify(x => x.Lose(), Times.Once);
        }

        [Test]
        public async Task MakeMove_AllConditionsFine_MakeMoveInvoked()
        {
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);

            _gameServiceMock
                .Verify(x => x.MakeMove(
                    It.IsAny<Game>(),
                    It.IsAny<GameFieldFields>()
                ),
                Times.Once
            );
        }

        //OnDisconnectedAsync
        [Test]
        public async Task OnDisconnectedAsync_GroupNotifiedOpponentDisconnected()
        {
            _itemsFake.Add(GameHub.RoomIdKey, _existingRoom.Id);
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                _initGameHub(db);
                await _gameHub.OnDisconnectedAsync(null);

                _responsesMock
                    .Verify(x => x.OpponentDisconnected(), Times.Once);
            }
        }

        [Test]
        public async Task OnDisconnectedAsync_RoomExist_RoomDestroyed()
        {
            _itemsFake.Add(GameHub.RoomIdKey, _existingRoom.Id);
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                db.Rooms.Add(_existingRoom);
                await db.SaveChangesAsync();
                int id = _existingRoom.Id;
                _initGameHub(db);

                await _gameHub.OnDisconnectedAsync(null);

                Assert.IsNull(db.Rooms.Find(id));
            }
        }

        private GameField _getFullField()
        {
            return new GameField {
                Down = "1",
                DownLeft = "1",
                DownRight = "1",
                Middle = "1",
                MiddleLeft = "1",
                MiddleRight = "1",
                Top = "1",
                TopLeft = "1",
                TopRight = "1"
            };
        }

        private async Task _makeMove_FieldIsNotEmptyScenario()
        {
            _gameServiceMock
                .Setup(x => x.CanMakeMove(It.IsAny<GameField>(), It.IsAny<GameFieldFields>()))
                .Returns(false);
            _addRoomIdToContextItemsAndInitGameHub();

            await _gameHub.MakeMove(1, 1, ValidPlayerId, ValidPassword);
        }

        private void _verifyGroupNeverGetPlayerMadeMove()
        {
            _responsesMock
               .Verify(x => x.PlayerMadeMove(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

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

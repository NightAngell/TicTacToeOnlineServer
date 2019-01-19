using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
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
    /// <summary>
    ///  <see cref="IRoomHubResponses"/>
    /// </summary>
    [TestFixture]
    class RoomHubTests : HubTestsBase<IRoomHubResponses, Db>
    {
        private const string GuestNick = "guestNick";
        private const int ExistingRoomId = 5;
        private const string ValidPassword = "ValidPassword";
        private const string InvalidPassword = "InvalidPassword";
        private const int NonExistingRoomId = 1;
        RoomHub _roomHub;
        Mock<IRoomService> _roomServiceMock;
        Room _lobbyRoom;
        readonly string _connectionId = "id1";

        [SetUp]
        public void SetUp()
        {
            BaseSetUp();
            _lobbyRoom = new Room() { HostNick = "nickhost" };
            _roomServiceMock = new Mock<IRoomService>();
            _roomServiceMockSetup();
        }

        private void _roomServiceMockSetup()
        {
            _roomServiceMock
               .Setup(x => x.AddRoomWithHostInsideWithInLobbyState(_lobbyRoom))
               .Callback(() => { _lobbyRoom.Id = ExistingRoomId; });
            _contextMock.Setup(x => x.ConnectionId).Returns(_connectionId);
            _contextMock.Setup(x => x.Items).Returns(_itemsFake);
            _roomServiceMock
               .Setup(x => x.GetRoomAsync(NonExistingRoomId))
               .ReturnsAsync((Room)null);
            _roomServiceMock
               .Setup(x => x.GetRoom(NonExistingRoomId))
               .Returns((Room)null);
            _roomServiceMock
                .Setup(x => x.GetRoomAsync(ExistingRoomId))
                .Callback(_initExistingInLobbyRoom)
                .ReturnsAsync(_lobbyRoom);
            _roomServiceMock
                .Setup(x => x.GetRoom(ExistingRoomId))
                .Callback(_initExistingInLobbyRoom)
                .Returns(_lobbyRoom);
        }
        
        private void _initExistingInLobbyRoom()
        {
            _lobbyRoom.Id = ExistingRoomId;
            _lobbyRoom.Password = ValidPassword;
            _lobbyRoom.NumberOfPlayersInside = 1;
            _lobbyRoom.State = RoomState.InLobby;
        }

        //CreateHostRoom
        [Test]
        public async Task CreateHostRoom_RoomAddedAndSaved()
        {
            await _initRoomHubAndCallCreateHostRoom();

            _roomServiceMock.Verify(
               x => x.AddRoomWithHostInsideWithInLobbyState(_lobbyRoom),
               Times.Once
            );
            _roomServiceMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateHostRoom_PlayerAddedToHubGroupAssociatedWithRoom()
        {
            await _initRoomHubAndCallCreateHostRoom();

            _groupsMock.Verify(
                x => x.AddToGroupAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task CreateHostRoom_ClientNotifiedAboudRoomCreated()
        {
            await _initRoomHubAndCallCreateHostRoom();

            _clientMock.Verify(x => x.HostRoomCreated(It.IsAny<int>()), Times.Once());
            _clientMock.Verify(x => x.HostRoomCreated(ExistingRoomId), Times.Once());
        }

        [Test]
        public async Task CreateHostRoom_RoomIdAddedToConnectionContextItems()
        {
            await _initRoomHubAndCallCreateHostRoom();

            object roomIdKey = RoomHub.roomIdKey as object;
            Assert.IsTrue(_contextMock.Object.Items.ContainsKey(roomIdKey));
        }

        private async Task _initRoomHubAndCallCreateHostRoom()
        {
            _initRoomHub(_dbMock.Object);
            await _roomHub.CreateHostRoom(_lobbyRoom);
        }

        //AddGuestToRoom
        [Test]
        public async Task AddGuestToRoom_RoomNotExist_SendToClientPlayerCannotJoinToRoomNotification()
        {
            _initRoomHub(_dbMock.Object);

            await _roomHub.AddGuestToRoom(NonExistingRoomId, ValidPassword, GuestNick);

            _verifyPlayerCannotJoinToRoomOnceCalledByClient();
        }

        [Test]
        public async Task AddGuestToRoom_RoomNotExist_GuestJoinToRoomNotificationNeverSendToClientOrOthersInGroup()
        {
            _initRoomHub(_dbMock.Object);

            await _roomHub.AddGuestToRoom(NonExistingRoomId, ValidPassword, GuestNick);

            _verifyPlayerGuestJoinToRoomNeverCalledByClientOrOthersInGroup();
        }

        [Test]
        public async Task AddGuestToRoom_PasswordIsIncorrect_SendToClientPlayerCannotJoinToRoomNotification()
        {
            _initRoomHub(_dbMock.Object);

            await _roomHub.AddGuestToRoom(ExistingRoomId, InvalidPassword, GuestNick);

            _verifyPlayerCannotJoinToRoomOnceCalledByClient();
        }

        [Test]
        public async Task AddGuestToRoom_PasswordIsIncorrect_GuestJoinToRoomNotificationNeverSendToClientOrOthersInGroup()
        {
            _initRoomHub(_dbMock.Object);

            await _roomHub.AddGuestToRoom(ExistingRoomId, InvalidPassword, GuestNick);

            _verifyPlayerGuestJoinToRoomNeverCalledByClientOrOthersInGroup();
        }
        
        [Test]
        public async Task AddGuestToRoom_RoomIsFull_SendToClientPlayerConnotJoinToRoomNotification()
        {
            _roomServiceMock
                .Setup(x => x.GetRoomAsync(ExistingRoomId))
                .Callback(_initExistingRoomForRoomIsFullScenario)
                .ReturnsAsync(_lobbyRoom);
            _roomServiceMock
                .Setup(x => x.GetRoom(ExistingRoomId))
                .Callback(_initExistingRoomForRoomIsFullScenario)
                .Returns(_lobbyRoom);
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            _verifyPlayerCannotJoinToRoomOnceCalledByClient();
        }

        [Test]
        public async Task AddGuestToRoom_RoomIsFull_GuestJoinToRoomNeverSendToClientOrOthersInGroup()
        {
            _roomServiceMock
                .Setup(x => x.GetRoomAsync(ExistingRoomId))
                .Callback(_initExistingRoomForRoomIsFullScenario)
                .ReturnsAsync(_lobbyRoom);
            _roomServiceMock
                .Setup(x => x.GetRoom(ExistingRoomId))
                .Callback(_initExistingRoomForRoomIsFullScenario)
                .Returns(_lobbyRoom);
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            _verifyPlayerGuestJoinToRoomNeverCalledByClientOrOthersInGroup();
        }

        private void _initExistingRoomForRoomIsFullScenario()
        {
            _lobbyRoom.Id = ExistingRoomId;
            _lobbyRoom.Password = ValidPassword;
            _lobbyRoom.NumberOfPlayersInside = 2;
            _lobbyRoom.State = RoomState.InLobby;
        }

        [Test]
        public async Task AddGuestToRoom_AllInputDataAreCorrectAndRoomIsNotFull_UserAddedToHubGroupAssociatedWithRoom()
        {
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            _groupsMock.Verify(x => x.AddToGroupAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Test]
        public async Task AddGuestToRoom_AllInputDataCorrectAndRoomIsNotFull_ClientAndOtherInRoomNotifiedGuestJoinToRoom()
        {
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            _clientMock.Verify(
                x => x.GuestJoinToRoom(It.IsAny<int>(), It.IsAny<string>()),
                Times.Once
            );
            _clientMock.Verify(
                x => x.GuestJoinToRoom(ExistingRoomId, It.IsAny<string>()),
                Times.Once
            );
            _responsesMock.Verify(
                x => x.GuestJoinToRoom(It.IsAny<int>(), It.IsAny<string>()),
                Times.Once
            );
            _responsesMock.Verify(
                x => x.GuestJoinToRoom(ExistingRoomId, It.IsAny<string>()),
                Times.Once
            );
        }

        [Test]
        public async Task AddGestToRoom_RoomIsNotFull_RoomIsFull()
        {
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            Assert.IsTrue(_lobbyRoom.NumberOfPlayersInside == 2);
        }

        [Test]
        public async Task AddGestToRoom_RoomIsNotFull_PlayersHaveAssignedId()
        {
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            Assert.IsTrue(_lobbyRoom.HostId.Length > 0);
            Assert.IsTrue(_lobbyRoom.GuestId.Length > 0);
        }

        [Test]
        public async Task AddGestToRoom_RoomIsNotFull_RoomStateChangedToWaitingForFirstPlayer()
        {
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            Assert.IsTrue(_lobbyRoom.State == RoomState.WaitingForFirstPlayer);
        }

        [Test]
        public async Task AddGestToRoom_RoomIsNotFull_RoomIsSaved()
        {
            await _initRoomHubAndCallAddGuestToRoomWithValidData();

            _roomServiceMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        private async Task _initRoomHubAndCallAddGuestToRoomWithValidData()
        {
            _initRoomHub(_dbMock.Object);

            await _roomHub.AddGuestToRoom(ExistingRoomId, ValidPassword, GuestNick);
        }

        private void _verifyPlayerCannotJoinToRoomOnceCalledByClient()
        {
            _clientMock.Verify(x => x.PlayerCannotJoinToRoom(It.IsAny<string>()), Times.Once);
        }

        private void _verifyPlayerGuestJoinToRoomNeverCalledByClientOrOthersInGroup()
        {
            _clientMock.Verify(
                x => x.GuestJoinToRoom(It.IsAny<int>(), It.IsAny<string>()),
                Times.Never
             );
            _clientsMock.Verify(x => x.OthersInGroup(It.IsAny<string>()), Times.Never);
        }

        //AbortRoom
        [Test]
        public async Task AbortRoom_ContextItemsNotContainRoomIdKey_NothingHappend()
        {
            _initRoomHub(_dbMock.Object);

            await _roomHub.AbortRoom();

            _roomServiceMock.Verify(x => x.AttachAndDestroyRoom(It.IsAny<int>()), Times.Never);
            _roomServiceMock.Verify(x => x.SaveChangesAsync(), Times.Never);
            _clientMock.Verify(x => x.RoomAborted(), Times.Never);
            _groupsMock.Verify(
                x => x.RemoveFromGroupAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task AbortRoom_ContextItemsContainRoomIdKey_RoomDestroyed()
        {
            _addRoomIdKeyToContextItems();
            _initRoomHub(_dbMock.Object);

            await _roomHub.AbortRoom();

            _roomServiceMock.Verify(x => x.AttachAndDestroyRoom(It.IsAny<int>()), Times.Once);
            _roomServiceMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AbortRoom_ContextItemsContainRoomIdKey_UserRemovedFromGroupAssociatedWithRoom()
        {
            _addRoomIdKeyToContextItems();
            _initRoomHub(_dbMock.Object);

            await _roomHub.AbortRoom();

            _groupsMock.Verify(
               x => x.RemoveFromGroupAsync(
                   It.IsAny<string>(),
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>()
               ),
               Times.Once
           );
        }

        [Test]
        public async Task AbortRoom_ContextItemsContainRoomIdKey_ClientNotifiedRoomAborted()
        {
            _addRoomIdKeyToContextItems();
            _initRoomHub(_dbMock.Object);

            await _roomHub.AbortRoom();

            _clientMock.Verify(x => x.RoomAborted(), Times.Once);
        }

        //OnDisconnectedAsync
        [Test]
        public async Task OnDisconnectedAsync_ContextItemsContainRoomIdKeyAndRoomExistAndIsInLobby_RoomRemoved()
        {
            _initExistingInLobbyRoom();
            _lobbyRoom.Id = default(int);
            int id;
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                db.Rooms.Add(_lobbyRoom);
                await db.SaveChangesAsync();
                id = _lobbyRoom.Id;
                _itemsFake.Add(RoomHub.roomIdKey, id);
                _initRoomHub(db);

                await _roomHub.OnDisconnectedAsync(null);

                var room = await db.Rooms.FindAsync(id);
                Assert.IsNull(room);
            }
        }

        [Test]
        [TestCase(RoomState.InGame)]
        [TestCase(RoomState.WaitingForFirstPlayer)]
        [TestCase(RoomState.WaitingForSecondPlayer)]
        public async Task OnDisconnectedAsync_ContextItemsContainRoomIdKeyAndRoomExistButNotInLobby_RoomNotRemoved(
            RoomState roomState
        )
        {
            _lobbyRoom.Id = default(int);
            _lobbyRoom.State = roomState;
            int id;
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                db.Rooms.Add(_lobbyRoom);
                await db.SaveChangesAsync();
                id = _lobbyRoom.Id;
                _itemsFake.Add(RoomHub.roomIdKey, id);
                _initRoomHub(db);

                await _roomHub.OnDisconnectedAsync(null);

                var room = await db.Rooms.FindAsync(id);
                Assert.IsNotNull(room);
            }
        }

        private void _addRoomIdKeyToContextItems()
        {
            _itemsFake.Add(RoomHub.roomIdKey, ExistingRoomId);
        }

        private void _initRoomHub(Db db)
        {
            _roomHub = new RoomHub(_roomServiceMock.Object, db) {
                Context = _contextMock.Object,
                Groups = _groupsMock.Object,
                Clients = _clientsMock.Object
            };
        } 
    }
}

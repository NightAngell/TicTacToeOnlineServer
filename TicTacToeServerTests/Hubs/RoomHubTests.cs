using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSupport.EfHelpers;
using TicTacToeServer.Database;
using TicTacToeServer.Hubs;
using TicTacToeServer.Hubs.Interfaces;
using TicTacToeServer.Models;
using TicTacToeServer.Services;

namespace TicTacToeServerTests.Hubs
{
    [TestFixture]
    class RoomHubTests
    {
        RoomHub _roomHub;
        Mock<IRoomService> _roomServiceMock;
        Mock<HubCallerContext> _contextMock;
        Mock<IGroupManager> _groupsMock;
        Mock<IHubCallerClients<IRoomHubResponses>> _clientsMock;
        Room _lobbyRoom;
        Dictionary<object, object> _itemsFake;
        Mock<IRoomHubResponses> _clientMock;
        Mock<Db> _dbMock = new Mock<Db>(new DbContextOptions<Db>());
        
        [SetUp]
        public void SetUp()
        {
            _lobbyRoom = new Room() { HostNick = "nickhost" };
            _roomServiceMock = new Mock<IRoomService>();
            _contextMock = new Mock<HubCallerContext>();
            _groupsMock = new Mock<IGroupManager>();
            _itemsFake = new Dictionary<object, object>();
            _clientMock = new Mock<IRoomHubResponses>();
            _clientsMock = new Mock<IHubCallerClients<IRoomHubResponses>>();
            _clientsMock.Setup(x => x.Caller).Returns(_clientMock.Object);
        }

        //CreateHostRoom
        [Test]
        public async Task CreateHostRoom_RoomAddedAndSaved()
        {
            await _arrangeAndActForCreateHostRoom();

            _roomServiceMock.Verify(
               x => x.AddRoomWithHostInsideWithInLobbyState(_lobbyRoom),
               Times.Once
            );
            _roomServiceMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateHostRoom_PlayerAddedToHubGroupAssociatedWithRoom()
        {
            await _arrangeAndActForCreateHostRoom();

            _groupsMock.Verify(
                x => x.AddToGroupAsync(
                    "id1",
                    It.IsAny<string>(),
                    default(CancellationToken)
                ),
                Times.Once
            );
        }

        [Test]
        public async Task CreateHostRoom_ClientNotifiedAboudRoomCreated()
        {
            await _arrangeAndActForCreateHostRoom();

            _clientMock.Verify(x => x.HostRoomCreated(5), Times.Once());
        }

        [Test]
        public async Task CreateHostRoom_RoomIdAddedToConnectionContextItems()
        {
            await _arrangeAndActForCreateHostRoom();

            object roomIdKey = RoomHub.roomIdKey as object;
            Assert.IsTrue(_contextMock.Object.Items.ContainsKey(roomIdKey));
        }

        private async Task _arrangeAndActForCreateHostRoom()
        {
            //Arrange
            _roomServiceMock
                .Setup(x => x.AddRoomWithHostInsideWithInLobbyState(_lobbyRoom))
                .Callback(() => { _lobbyRoom.Id = 5; });
            _contextMock.Setup(x => x.ConnectionId).Returns("id1");
            _contextMock.Setup(x => x.Items).Returns(_itemsFake);
            InitRoomHub(_dbMock.Object);

            //Act
            await _roomHub.CreateHostRoom(_lobbyRoom);
        }

        //AddGuestToRoom
        [Test]
        public async Task AddGuestToRoom

        private void InitRoomHub(Db db)
        {
            _roomHub = new RoomHub(_roomServiceMock.Object, db) {
                Context = _contextMock.Object,
                Groups = _groupsMock.Object,
                Clients = _clientsMock.Object
            };
        }
    }
}

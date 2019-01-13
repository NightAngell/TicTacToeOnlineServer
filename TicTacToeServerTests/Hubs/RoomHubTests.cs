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
        public async Task CreateHostRoom_HostRoomCreatedWithHostInsideAndUserNotifiedAboutId()
        {               
            _roomServiceMock
                .Setup(x => x.AddRoomWithHostInsideWithInLobbyState(_lobbyRoom))
                .Callback(()=> { _lobbyRoom.Id = 5; });
            _contextMock.Setup(x => x.ConnectionId).Returns("id1");
            _contextMock.Setup(x => x.Items).Returns(_itemsFake);

            InitRoomHub(_dbMock.Object);
            await _roomHub.CreateHostRoom(_lobbyRoom);

            _roomServiceMock.Verify(
                x => x.AddRoomWithHostInsideWithInLobbyState(_lobbyRoom),
                Times.Once
            );
            _clientMock.Verify(x => x.HostRoomCreated(5), Times.Once());
        }

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

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicTacToeServer.Controllers;
using TicTacToeServer.DTO;
using TicTacToeServer.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToeServerTests.Controllers
{
    [TestFixture]
    class RoomControllerTests
    {
        RoomController _roomController;
        Mock<IRoomService> _roomServiceMock;

        [SetUp]
        public void SetUp()
        {
            _roomServiceMock = new Mock<IRoomService>(); 
        }

        static object[] _roomDtosWithCount = {
             new object[] { _getRoomDtos(0), 0 },
             new object[] { _getRoomDtos(1), 1 },
             new object[] { _getRoomDtos(2), 2 },
             new object[] { _getRoomDtos(3), 3 },
        };

        [Test, TestCaseSource(nameof(_roomDtosWithCount))]
        public async Task GetListOfRooms_WeHave3Rooms_WeGet3Dtos(
            List<RoomDto> roomDtos,
            int numberOfDtos
        )
        {
            _roomServiceMock.Setup(r => r.GetListOfRoomsDtosInLobbyAsync()).Returns(
                Task.FromResult((IEnumerable<RoomDto>)roomDtos)
            );
            _roomController = new RoomController(_roomServiceMock.Object);

            var result = (await _roomController.GetListOfRooms()).Result as ObjectResult;
            if (result == null) Assert.IsTrue(false);
            var listOfRoomDtos = result.Value as List<RoomDto>;
            Assert.IsTrue(listOfRoomDtos.Count() == numberOfDtos);
        }

        private static List<RoomDto> _getRoomDtos(int numberOfDtos)
        {
            var roomDtos = new List<RoomDto>();
            for(var i = 0; i < numberOfDtos; i++)
            {
                roomDtos.Add(new RoomDto());
            }

            return roomDtos;
        }
    }
}

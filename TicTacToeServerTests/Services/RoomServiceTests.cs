using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TestSupport.EfHelpers;
using TicTacToeServer.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TicTacToeServer.Models;
using TicTacToeServer.Services;
using Moq;
using AutoMapper;
using System.Threading.Tasks;
using TicTacToeServer.DTO;
using TicTacToeServer.Enums;
using System.Threading;

namespace TicTacToeServerTests.Services
{
    /// <summary>
    /// https://github.com/JonPSmith/EfCore.TestSupport/wiki/1.-Sqlite-in-memory-test-database
    /// </summary>
    [TestFixture]
    class RoomServiceTests
    {
        RoomService _roomService;
        Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
        }

        //GetListOfRoomsAsync
        private static object[] _roomsWithCount =
        {
            new object[] {
                new List<Room> {
                },
                0
            },
            new object[] {
                new List<Room> {
                    new Room { HostNick = "H" }
                },
                1
            },
            new object[] {
                new List<Room> {
                    new Room { HostNick = "H" },
                    new Room { HostNick = "H" }
                },
                2
            },
            new object[] {
                new List<Room> {
                    new Room { HostNick = "H" },
                    new Room { HostNick = "H" },
                    new Room { HostNick = "H" }
                },
                3
            },
        };

        [Test, TestCaseSource(nameof(_roomsWithCount))]
        public async Task GetListOfRoomsAsync_DbContainRooms_WeGetAllRooms(
            List<Room> rooms,
            int numberOfRooms
        )
        {
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                db.Rooms.AddRange(rooms);
                db.SaveChanges();

                _roomService = new RoomService(db, _mapperMock.Object);
                var listOfRooms = await _roomService.GetListOfRoomsAsync();
                Assert.IsTrue(listOfRooms.Count() == numberOfRooms);
            }
        }

        //GetListOfRoomsDtosInLobbyAsync
        [Test]
        [TestCase("", false)]
        [TestCase("passWord123", true)]
        public async Task GetListOfRoomsDtosInLobbyAsync_WeHaveOneRoom_WeGetDtoWithIsPasswordSetProperly(
            string password,
            bool isPassword
        )
        {
            string hostNick = "PasswordOwner";
            var room = new Room {
                HostNick = hostNick,
                Password = password
            };
            
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                db.Rooms.Add(room);
                await db.SaveChangesAsync();
                _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(
                    new RoomDto
                    {
                        Id = room.Id,
                        HostNick = hostNick,
                        IsPassword = isPassword
                    }
                );

                _roomService = new RoomService(db, _mapperMock.Object);
                var roomDtos = await _roomService.GetListOfRoomsDtosInLobbyAsync();
                var roomDto = roomDtos.First();
                Assert.IsTrue(roomDto.IsPassword == isPassword);
            }
        }

        [Test]
        public async Task GetListOfRoomsDtosInLobbyAsync_WeHaveThreeRoomsInLobby_WeGetThreeDtos()
        {
            var room = new Room { HostNick = "3" };
            var room2 = new Room { HostNick = "3" };
            var room3 = new Room { HostNick = "3" };
            var room4 = new Room { HostNick = "3", State = RoomState.InGame };
            var room5 = new Room { HostNick = "3", State = RoomState.WaitingForFirstPlayer };
            var room6 = new Room { HostNick = "3", State = RoomState.WaitingForSecondPlayer };
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                db.Rooms.Add(room);
                db.Rooms.Add(room2);
                db.Rooms.Add(room3);
                db.Rooms.Add(room4);
                db.Rooms.Add(room5);
                db.Rooms.Add(room6);
                await db.SaveChangesAsync();

                _mapperMock.Setup(m => m.Map<RoomDto>(It.IsAny<RoomDto>())).Returns(
                    new RoomDto()    
                );

                _roomService = new RoomService(db, _mapperMock.Object);
                var rooms = await _roomService.GetListOfRoomsDtosInLobbyAsync();

                Assert.IsTrue(rooms.Count() == 3);
            }
        }

        //AddRoom
        [Test]
        public async Task AddRoom_WeHaveValidRoom_RoomAddedToDb()
        {
            var room = new Room{
                HostNick = "Abc"
            };
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                db.Rooms.Add(room);
                await db.SaveChangesAsync();

                var roomFromDb = await db.Rooms.FirstOrDefaultAsync();
                Assert.IsTrue(roomFromDb != null);
            }
        }

        [Test]
        public void AddRoom_WeHaveInvalidRoom_ThrowsDbUpdateException()
        {
            var room = new Room();
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                _roomService = new RoomService(db, _mapperMock.Object);
                _roomService.AddRoom(room);
                
                Assert.Throws<DbUpdateException>(() => db.SaveChanges());
            }
        }

        //AddRoomWithHostInsideWithInLobbyState
        [Test]
        public async Task AddRoomWithHostInsideWithInLobbyState_WeHaveRoom_RoomAddedWithRequiredValues()
        {
            string hostNick = "aa";
            var room = new Room() { HostNick = hostNick };
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                _roomService = new RoomService(db, _mapperMock.Object);

                _roomService.AddRoomWithHostInsideWithInLobbyState(room);
                db.SaveChanges();

                Assert.IsTrue(room.HostNick == hostNick);
                Assert.IsTrue(room.NumberOfPlayersInside == 1);
                Assert.IsTrue(room.State == RoomState.InLobby);

                var roomFromDb = await db.Rooms.FirstOrDefaultAsync();
                Assert.IsTrue(roomFromDb != null);
            }
        }

        //DestroyRoom
        [Test]
        public void DestroyRoom_RoomExist_RoomRemoved()
        {
            var room = new Room { HostNick = "aa" };
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                db.Rooms.Add(room);
                db.SaveChanges();

                _roomService = new RoomService(db, _mapperMock.Object);

                db.Entry(room).State = EntityState.Detached;
                _roomService.AttachAndDestroyRoom(room.Id);
                db.SaveChanges();

                Assert.IsTrue(db.Rooms.Find(room.Id) == null);
            }
        }

        [Test]
        public void DestroyRoom_RoomNotExist_ThrowsDbUpdateConcurrencyException()
        {
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                _roomService = new RoomService(db, _mapperMock.Object);
                _roomService.AttachAndDestroyRoom(5);
                
                Assert.Throws<DbUpdateConcurrencyException>(()=> db.SaveChanges());
            }
        }

        //GetRoomAsync
        [Test]
        public async Task GetRoomAsync_RoomExist_WeGetRoom()
        {
            var room = new Room { HostNick = "nick" };
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                db.Rooms.Add(room);
                await db.SaveChangesAsync();
                _roomService = new RoomService(db, _mapperMock.Object);

                var roomFromDb = await _roomService.GetRoomAsync(room.Id);
                Assert.IsNotNull(roomFromDb);
            }
        }

        [Test]
        public async Task GetRoomAsync_RoomNotExist_returnNull()
        {
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                _roomService = new RoomService(db, _mapperMock.Object);

                var roomFromDb = await _roomService.GetRoomAsync(5);
                Assert.IsNull(roomFromDb);
            }
        }

        //GetRoomWithGameAndGameField
        [Test]
        public async Task GetRoomWithGameAndGameField_RoomWithGameAndFieldExist_WeGetRoom()
        {
            var room = new Room { HostNick = "nick" };
            room.InitGame();
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                db.Rooms.Add(room);
                await db.SaveChangesAsync();
                _roomService = new RoomService(db, _mapperMock.Object);

                var roomFromDb = await _roomService.GetRoomAsync(room.Id);
                Assert.IsNotNull(roomFromDb);
                Assert.IsNotNull(roomFromDb.Game);
                Assert.IsNotNull(roomFromDb.Game.Field);
            }
        }

        [Test]
        public async Task GetRoomWithGameAndGameField_RoomExistButGameIsNotInitYet_WeGetRoomWithoutGameAndField()
        {
            var room = new Room { HostNick = "nick" };
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                await db.Database.EnsureCreatedAsync();
                db.Rooms.Add(room);
                await db.SaveChangesAsync();
                _roomService = new RoomService(db, _mapperMock.Object);

                var roomFromDb = await _roomService.GetRoomAsync(room.Id);
                Assert.IsNotNull(roomFromDb);
                Assert.IsNull(roomFromDb.Game);
            }
        }

        //GetRoom
        [Test]
        public void GetRoom_RoomExist_WeGetRoom()
        {
            var room = new Room { HostNick = "nick" };
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                db.Rooms.Add(room);
                db.SaveChanges();
                _roomService = new RoomService(db, _mapperMock.Object);

                var roomFromDb = _roomService.GetRoom(room.Id);
                Assert.IsNotNull(roomFromDb);
            }
        }

        [Test]
        public void GetRoom_RoomNotExist_returnNull()
        {
            using (var db = new Db(SqliteInMemory.CreateOptions<Db>()))
            {
                db.Database.EnsureCreated();
                _roomService = new RoomService(db, _mapperMock.Object);

                var roomFromDb = _roomService.GetRoom(5);
                Assert.IsNull(roomFromDb);
            }
        }

        //SaveChanges
        [Test]
        public void SaveChanges_SaveChangesOnDbCalledOnce()
        {
            var dbMock = new Mock<Db>(SqliteInMemory.CreateOptions<Db>());
            _roomService = new RoomService(dbMock.Object , _mapperMock.Object);
            _roomService.SaveChanges();

            dbMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        //SaveChangesAsync
        [Test]
        public async Task SaveChangesAsync_SaveChangesAsyncOnDbCalledOnce()
        {
            var dbMock = new Mock<Db>(SqliteInMemory.CreateOptions<Db>());
            _roomService = new RoomService(dbMock.Object, _mapperMock.Object);
            await _roomService.SaveChangesAsync();

            dbMock.Verify(x => x.SaveChangesAsync(default(CancellationToken)), Times.Once);
        }
    }
}

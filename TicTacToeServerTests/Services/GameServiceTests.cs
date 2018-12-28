using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicTacToeServer.Enums;
using TicTacToeServer.Exceptions;
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

        #region gameFieldsHorizontalWinner
        private static GameField _gameFieldWinnerHorizontalTop()
        {
            return new GameField {
                TopLeft = "x",
                Top = "x",
                TopRight = "x"
            };
        }

        private static GameField _gameFieldWinnerHorizontalMiddle()
        {
            return new GameField
            {
                MiddleLeft = "x",
                Middle = "x",
                MiddleRight = "x"
            };
        }

        private static GameField _gameFieldWinnerHorizontalDown()
        {
            return new GameField
            {
                DownLeft = "x",
                Down = "x",
                DownRight = "x"
            };
        }

        private static object[] _gameFieldsHorizontalWinner = {
            new object[] { _gameFieldWinnerHorizontalTop() },
            new object[] { _gameFieldWinnerHorizontalMiddle() },
            new object[] { _gameFieldWinnerHorizontalDown() },
        };
        #endregion

        #region gameFieldsVerticalWinner
        private static GameField _gameFieldWinnerVerticalLeft()
        {
            return new GameField
            {
                DownLeft = "x",
                MiddleLeft = "x",
                TopLeft = "x"
            };
        }

        private static GameField _gameFieldWinnerVerticalMiddle()
        {
            return new GameField
            {
                Top = "x",
                Middle = "x",
                Down = "x"
            };
        }

        private static GameField _gameFieldWinnerVerticalRight()
        {
            return new GameField
            {
                TopRight = "x",
                MiddleRight = "x",
                DownRight = "x"
            };
        }

        private static object[] _gameFieldsVerticalWinner = {
            new object[] { _gameFieldWinnerVerticalLeft() },
            new object[] { _gameFieldWinnerVerticalMiddle() },
            new object[] { _gameFieldWinnerVerticalRight() },
        };
        #endregion

        #region gameFieldsUnderTheSlantWinner
        private static GameField _gameFieldTopLeftDownRightWinner()
        {
            return new GameField
            {
                TopLeft = "x",
                Middle = "x",
                DownRight = "x"
            };
        }

        private static GameField _gameFieldDownLeftTopRightWinner()
        {
            return new GameField
            {
                DownLeft = "x",
                Middle = "x",
                TopRight = "x"
            };
        }

        private static object[] _gameFieldsUnderTheSlantWinner = {
            new object[] { _gameFieldTopLeftDownRightWinner() },
            new object[] { _gameFieldDownLeftTopRightWinner() },
        };
        #endregion

        private static object[] _gameFieldsWhereIsWinner =
        {
            new object[] { _gameFieldWinnerHorizontalTop() },
            new object[] { _gameFieldWinnerHorizontalMiddle() },
            new object[] { _gameFieldWinnerHorizontalDown() },

            new object[] { _gameFieldWinnerVerticalLeft() },
            new object[] { _gameFieldWinnerVerticalMiddle() },
            new object[] { _gameFieldWinnerVerticalRight() },

            new object[] { _gameFieldTopLeftDownRightWinner() },
            new object[] { _gameFieldDownLeftTopRightWinner() }
        };
       
        [SetUp]
        public void SetUp()
        {
            _roomServiceMock = new Mock<IRoomService>();

            _gameService = new GameService(_roomServiceMock.Object);
        }

        //ValidatePlayer
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

        //IsWinner
        [Test]
        public void IsWinner_FieldIsEmpty_ReturnsFalse()
        {
            var emptyField = new GameField();
            Assert.IsFalse(_gameService.IsWinner(emptyField));
        }

        [Test, TestCaseSource(nameof(_gameFieldsWhereIsWinner))]
        public void IsWinner_IsWinner_ReturnsTrue(GameField field)
        {
            Assert.IsTrue(_gameService.IsWinner(field));
        }

        [Test]
        public void IsWinner_PartOfFieldsNotEmpty_ReturnsFalse()
        {
            var field = new GameField {
                TopLeft = "x",
                TopRight = "x",
                Down = "o",
                DownLeft = "o"
            };

            Assert.IsFalse(_gameService.IsWinner(field));
        }

        [Test]
        public void IsWinner_XonTopAndBottomOMiddle_ReturnsFalse()
        {
            var field = new GameField
            {
                Top = "x",
                Middle = "0",
                Down = "x",
            };

            Assert.IsFalse(_gameService.IsWinner(field));
        }

        //CanMakeMove
        [Test]
        public void CanMakeMove_FieldIsFree_ReturnsTrue()
        {
            var field = new GameField();
            Assert.IsTrue(_gameService.CanMakeMove(field, GameFieldFields.Down));
        }

        [Test]
        public void CanMakeMove_FieldNotFree_ReturnsFalse()
        {
            var field = new GameField();
            field.Down = "x";
            Assert.IsFalse(_gameService.CanMakeMove(field, GameFieldFields.Down));
        }

        //Map2DimensionalParamsToGameSimpleField
        [Test]
        public void Map2DimensionalParamsToGameSimpleField_ParamsInvalid_ThrowsNotValidFieldParamsException()
        {
            Assert.Throws<NotValidFieldParamsException>(
                ()=> _gameService.Map2DimensionalParamsToGameSimpleField(1, 5)
            );
        }

        private static object[] _paramsToMapWithResult =
        {
            new object[] { 0, 0, GameFieldFields.TopLeft},
            new object[] { 0, 1, GameFieldFields.Top},
            new object[] { 0, 2, GameFieldFields.TopRight},

            new object[] { 1, 0, GameFieldFields.MiddleLeft},
            new object[] { 1, 1, GameFieldFields.Middle},
            new object[] { 1, 2, GameFieldFields.MiddleRight},

            new object[] { 2, 0, GameFieldFields.DownLeft},
            new object[] { 2, 1, GameFieldFields.Down},
            new object[] { 2, 2, GameFieldFields.DownRight},

        };
        [Test, TestCaseSource(nameof(_paramsToMapWithResult))]
        public void Map2DimensionalParamsToGameSimpleField_ParamsValid_AllParamsCorrectlyMapped(
            int i,
            int j,
            GameFieldFields field
        )
        {
            Assert.IsTrue(_gameService.Map2DimensionalParamsToGameSimpleField(i,j) == field);
        }

        //MakeMove
        [Test]
        public void MakeMove_MoveAlwaysShouldBeDone_MoveDone()
        {
            var game = new Game();
            game.CurrentPlayerId = "playerTestID";
            game.Field = new GameField();

            _gameService.MakeMove(game, GameFieldFields.Middle);
            Assert.IsTrue(game.Field.Middle == "playerTestID");
        }

        //NextPlayerTurn
        [Test]
        public void NextPlayerTurn_WeHaveValidData_CurrentPlayerChange()
        {
            var room = new Room() {
                GuestId = "a",
                HostId = "b"
            };
            room.Game = new Game {
                CurrentPlayerId = "a"
            };

            _gameService.NextPlayerTurn(room);

            Assert.IsTrue(room.Game.CurrentPlayerId == "b");
        }

        //IsPlayerTurn
        [Test]
        public void IsPlayerTurn_IsPlayerTurn_ReturnsTrue()
        {
            string playerId = "playerTestID";
            var game = new Game();
            game.CurrentPlayerId = playerId;

            Assert.IsTrue(_gameService.IsPlayerTurn(game, playerId));
        }

        [Test]
        public void IsPlayerTurn_IsNotPlayerTurn_ReturnsFalse()
        {
            string playerId = "playerTestID";
            var game = new Game();
            game.CurrentPlayerId = playerId;

            Assert.IsFalse(_gameService.IsPlayerTurn(game, playerId));
        }

        //IsWinnerVertical
        [Test, TestCaseSource(nameof(_gameFieldsVerticalWinner))]
        public void IsWinnerVertical_IsWinnerVertical_ReturnsTrue(GameField field)
        {
            Assert.IsTrue(_gameService.IsWinnerVertical(field));
        }

        [Test, TestCaseSource(nameof(_gameFieldsHorizontalWinner))]
        public void IsWinnerVertical_IsWinnerHorizontal_ReturnsFalse(GameField field)
        {
            Assert.IsFalse(_gameService.IsWinnerVertical(field));
        }

        [Test, TestCaseSource(nameof(_gameFieldsUnderTheSlantWinner))]
        public void IsWinnerVertical_IsWinnerUnderTheSlant_ReturnsFalse(GameField field)
        {
            Assert.IsFalse(_gameService.IsWinnerVertical(field));
        }

        //IsWinnerHorizontal
        [Test, TestCaseSource(nameof(_gameFieldsHorizontalWinner))]
        public void IsWinnerHorizontal_IsWinnerHorizontal_ReturnsTrue(GameField field)
        {
            Assert.IsTrue(_gameService.IsWinnerHorizontal(field));
        }

        [Test, TestCaseSource(nameof(_gameFieldsVerticalWinner))]
        public void IsWinnerHorizontal_IsWinnerVertical_ReturnsFalse(GameField field)
        {
            Assert.IsFalse(_gameService.IsWinnerHorizontal(field));
        }

        [Test, TestCaseSource(nameof(_gameFieldsUnderTheSlantWinner))]
        public void IsWinnerHorizontal_IsWinnerUnderTheSlant_ReturnsFalse(GameField field)
        {
            Assert.IsFalse(_gameService.IsWinnerHorizontal(field));
        }

        //IsWinnerUnderTheSlant
        [Test, TestCaseSource(nameof(_gameFieldsUnderTheSlantWinner))]
        public void IIsWinnerUnderTheSlant_IsWinnerUnderTheSlant_ReturnsTrue(GameField field)
        {
            Assert.IsTrue(_gameService.IsWinnerUnderTheSlant(field));
        }

        [Test, TestCaseSource(nameof(_gameFieldsVerticalWinner))]
        public void IIsWinnerUnderTheSlant_IsWinnerVertical_ReturnsFalse(GameField field)
        {
            Assert.IsFalse(_gameService.IsWinnerUnderTheSlant(field));
        }

        [Test, TestCaseSource(nameof(_gameFieldsHorizontalWinner))]
        public void IIsWinnerUnderTheSlant_IsWinnerHorizontal_ReturnsFalse(GameField field)
        {
            Assert.IsFalse(_gameService.IsWinnerUnderTheSlant(field));
        }

        //SaveChangesAsync
        [Test]
        public async Task SaveChangesAsync_MethodFired_RoomServiceSaveChangesAsyncFiredOnce()
        {
            await _gameService.SaveChangesAsync();
            _roomServiceMock.Verify(s => s.SaveChangesAsync(), Times.Once);
        }
    }
}

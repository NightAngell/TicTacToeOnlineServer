using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}

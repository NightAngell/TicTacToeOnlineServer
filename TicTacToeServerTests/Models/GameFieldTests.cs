using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeServer.Models;

namespace TicTacToeServerTests.Models
{
    [TestFixture]
    class GameFieldTests
    {
        private static GameField _emptyGameField = new GameField();
        private static object[] _notFullFields = new object[] {
            _getFieldWithEmptyDown(),
            _getFieldWithEmptyDownLeft(),
            _getFieldWithEmptyDownRight(),

            _getFieldWithEmptyMiddle(),
            _getFieldWithEmptyMiddleLeft(),
            _getFieldWithEmptyMiddleRight(),

            _getFieldWithEmptyTop(),
            _getFieldWithEmptyTopLeft(),
            _getFieldWithEmptyTopRight(),

            _emptyGameField
        };

        [Test]
        public void IsFull_RoomIsFull_ReturnsTrue()
        {
            Assert.IsTrue(_getFullField().IsFull);
        }

        [Test]
        [TestCaseSource(nameof(_notFullFields))]
        public void IsFull_RoomIsNotFull_ReturnsFalse(GameField field)
        {
            Assert.IsFalse(field.IsFull);
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
                TopRight = "1",
                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyDown()
        {
            return new GameField
            {
                DownLeft = "1",
                DownRight = "1",
                Middle = "1",
                MiddleLeft = "1",
                MiddleRight = "1",
                TopRight = "1",
                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyDownLeft()
        {
            return new GameField
            {
                Down = "1",
                DownRight = "1",
                Middle = "1",
                MiddleLeft = "1",
                MiddleRight = "1",
                TopRight = "1",
                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyDownRight()
        {
            return new GameField
            {
                Down = "1",
                DownLeft = "1",

                Middle = "1",
                MiddleLeft = "1",
                MiddleRight = "1",
                TopRight = "1",
                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyMiddle()
        {
            return new GameField
            {
                Down = "1",
                DownLeft = "1",
                DownRight = "1",

                MiddleLeft = "1",
                MiddleRight = "1",
                TopRight = "1",
                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyMiddleLeft()
        {
            return new GameField
            {
                Down = "1",
                DownLeft = "1",
                DownRight = "1",
                Middle = "1",

                MiddleRight = "1",
                TopRight = "1",
                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyMiddleRight()
        {
            return new GameField
            {
                Down = "1",
                DownLeft = "1",
                DownRight = "1",
                Middle = "1",
                TopRight = "1",
                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyTopRight()
        {
            return new GameField
            {
                Down = "1",
                DownLeft = "1",
                DownRight = "1",
                Middle = "1",
                MiddleLeft = "1",
                MiddleRight = "1",

                Top = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyTop()
        {
            return new GameField
            {
                Down = "1",
                DownLeft = "1",
                DownRight = "1",
                Middle = "1",
                MiddleLeft = "1",
                MiddleRight = "1",
                TopRight = "1",
                TopLeft = "1",
            };
        }

        private static GameField _getFieldWithEmptyTopLeft()
        {
            return new GameField
            {
                Down = "1",
                DownLeft = "1",
                DownRight = "1",
                Middle = "1",
                MiddleLeft = "1",
                MiddleRight = "1",
                TopRight = "1",
                Top = "1",
            };
        }

    }
}

using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeServer.Enums;
using TicTacToeServer.Exceptions;
using TicTacToeServer.Models;

namespace TicTacToeServer.Services
{
    public class GameService : IGameService
    {
        readonly IRoomService roomService;

        public GameService(IRoomService roomService)
        {
            this.roomService = roomService;
        }

        public bool ValidatePlayer(Room room, string playerId, string password)
        {
            return (room.HostId == playerId && room.Password == password)
                   || (room.GuestId == playerId && room.Password == password);
        }

        public bool IsWinner(GameField field)
        {
            return IsWinnerUnderTheSlant(field) || IsWinnerHorizontal(field) || IsWinnerVertical(field);
        }

        public bool CanMakeMove(GameField field, GameFieldFields simpleField)
        {
            string fieldValue = (string)field.GetType()
                                .GetProperty(simpleField.ToString())
                                .GetValue(field, null);

            return string.IsNullOrEmpty(fieldValue);
        }

        /// <exception cref="NotValidFieldParamsException">
        ///     Connot convert that params to simple field
        /// </exception>
        public GameFieldFields Map2DimensionalParamsToGameSimpleField(int i, int j)
        {
            if (i == 0 && j == 0) return GameFieldFields.TopLeft;
            if (i == 0 && j == 1) return GameFieldFields.Top;
            if (i == 0 && j == 2) return GameFieldFields.TopRight;

            if (i == 1 && j == 0) return GameFieldFields.MiddleLeft;
            if (i == 1 && j == 1) return GameFieldFields.Middle;
            if (i == 1 && j == 2) return GameFieldFields.MiddleRight;

            if (i == 2 && j == 0) return GameFieldFields.DownLeft;
            if (i == 2 && j == 1) return GameFieldFields.Down;
            if (i == 2 && j == 2) return GameFieldFields.DownRight;

            throw new NotValidFieldParamsException();
        }

        public void MakeMove(Game game, GameFieldFields simpleField)
        {
            GameField field = game.Field;
            field
                .GetType()
                .GetProperty(simpleField.ToString())
                .SetValue(field, game.CurrentPlayerId);
        }

        public void NextPlayerTurn(Room room)
        {
            if (room.Game.CurrentPlayerId == room.HostId)
                room.Game.CurrentPlayerId = room.GuestId;
            else
                room.Game.CurrentPlayerId = room.HostId;
        }

        public bool IsPlayerTurn(Game game, string playerId)
        {
            return game.CurrentPlayerId == playerId;
        }

        public bool IsWinnerVertical(GameField field)
        {
            bool firstLineFromLeft = _sameStringValueInThreeVariables(field.TopLeft, field.MiddleLeft, field.DownLeft);
            bool secondLineFromLeft = _sameStringValueInThreeVariables(field.Top, field.Middle, field.Down);
            bool thirdLineFromLeft = _sameStringValueInThreeVariables(field.TopRight, field.MiddleRight, field.DownRight);

            return firstLineFromLeft || secondLineFromLeft || thirdLineFromLeft;
        }

        public bool IsWinnerHorizontal(GameField field)
        {
            bool firstLineFromTop = _sameStringValueInThreeVariables(field.TopLeft, field.Top, field.TopRight);
            bool secondLineFromTop = _sameStringValueInThreeVariables(field.MiddleLeft, field.Middle, field.MiddleRight);
            bool thirdLineFromTop = _sameStringValueInThreeVariables(field.DownLeft, field.Down, field.DownRight);

            return firstLineFromTop || secondLineFromTop || thirdLineFromTop;
        }

        public bool IsWinnerUnderTheSlant(GameField field)
        {
            bool fromDownLeftToTopRight = _sameStringValueInThreeVariables(field.Middle, field.DownLeft, field.TopRight);
            bool fromTopLeftToDownRight = _sameStringValueInThreeVariables(field.Middle, field.TopLeft, field.DownRight);

            return fromDownLeftToTopRight || fromTopLeftToDownRight;
        }

        public async Task SaveChangesAsync()
        {
            await roomService.SaveChangesAsync();
        }

        private bool _sameStringValueInThreeVariables(string first, string second, string third)
        {
            if (!string.IsNullOrEmpty(first)
                && !string.IsNullOrEmpty(second)
                && !string.IsNullOrEmpty(third))
            {
                return first == second && first == third;
            }

            return false;
        }
    }
}

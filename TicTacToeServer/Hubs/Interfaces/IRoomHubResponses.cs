using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeServer.Hubs.Interfaces
{
    public interface IRoomHubResponses : IHubResponsesConstraint
    {
        Task HostRoomCreated(int roomId);
        Task PlayerCannotJoinToRoom(string reason);
        Task GuestJoinToRoom(int roomId, string guid);
        Task RoomAborted();
    }
}

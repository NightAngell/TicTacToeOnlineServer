using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TicTacToeServer.Hubs.Interfaces;

namespace TicTacToeServerTests.Hubs
{
    /// <summary>
    /// You must provide [SetUp](if you using Nunit) and call BaseSetUp if you want us this class
    /// <para>You must assign context, groups and clients mocks object when create hub to tests.</para>
    /// <list type="bullet">
    /// <item>Context = _contextMock.Object, </item>
    /// <item>Groups = _groupsMock.Object, </item>
    /// <item>Clients = _clientsMock.Object</item>
    /// </list>
    /// </summary>
    abstract class HubTestsBase<TIHubResponses, TDbContext>
        where TDbContext : DbContext
        where TIHubResponses : class, IHubResponsesConstraint
    {
        protected Mock<HubCallerContext> _contextMock;
        protected Mock<IGroupManager> _groupsMock;
        protected Mock<IHubCallerClients<TIHubResponses>> _clientsMock;
        protected Dictionary<object, object> _itemsFake;
        protected Mock<TDbContext> _dbMock = new Mock<TDbContext>(new DbContextOptions<TDbContext>());
        protected Mock<TIHubResponses> _clientMock;
        protected Mock<TIHubResponses> _responsesMock;

        protected void BaseSetUp()
        {
            _contextMock = new Mock<HubCallerContext>();
            _groupsMock = new Mock<IGroupManager>();
            _itemsFake = new Dictionary<object, object>();
            _clientMock = new Mock<TIHubResponses>();
            _clientsMock = new Mock<IHubCallerClients<TIHubResponses>>();
            _responsesMock = new Mock<TIHubResponses>();
            _contextMock.Setup(x => x.Items).Returns(_itemsFake);
            _clientsMock.Setup(x => x.Caller).Returns(_clientMock.Object);
            _clientsMock
                .Setup(x => x.OthersInGroup(It.IsAny<string>()))
                .Returns(_responsesMock.Object);
            _clientsMock
                .Setup(x => x.Group(It.IsAny<string>()))
                .Returns(_responsesMock.Object);
        }

        protected void _verifySomebodyAddedToGroup(Times times)
        {
            _groupsMock
                .Verify(x => x.AddToGroupAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                    times
                );
        }

        protected void _verifySomebodyAddedToGroup(Times times, string groupName)
        {
            _groupsMock
                .Verify(x => x.AddToGroupAsync(
                    It.IsAny<string>(),
                    groupName,
                    It.IsAny<CancellationToken>()),
                    times
                );
        }

        protected void _verifySomebodyAddedToGroup(Times times, string groupName, string connectionId)
        {
            _groupsMock
                .Verify(x => x.AddToGroupAsync(
                    connectionId,
                    groupName,
                    It.IsAny<CancellationToken>()),
                    times
                );
        }

        protected void _verifyContextItemsContainKeyValuePair(object key, object value)
        {
            Assert.IsTrue(_itemsFake.ContainsKey(key));
            Assert.IsTrue(_itemsFake.ContainsValue(value));
        }
    }
}

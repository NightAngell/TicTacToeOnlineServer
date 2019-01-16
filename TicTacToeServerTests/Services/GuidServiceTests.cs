using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeServer.Services;

namespace TicTacToeServerTests.Services
{
    [TestFixture]
    class GuidServiceTests
    {
        GuidService _guidService = new GuidService();

        [Test]
        public void NewGuid_WeGetNewGuid()
        {
            var guid = _guidService.NewGuid();
            Assert.IsTrue(guid != null);
        }

    }
}

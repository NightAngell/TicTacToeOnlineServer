using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TicTacToeServer.Enums;
using TicTacToeServer.Services;

namespace TicTacToeServerTests.Services
{
    [TestFixture]
    class JwtTokenServiceTests
    {
        Mock<IConfiguration> _configMock;
        JwtTokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            _configMock = new Mock<IConfiguration>();
            _configMock
                .Setup(x => x["Jwt:SigningKey"])
                .Returns("1p2p3laa34dfjadskfhdjskfhsdfklhaklhadfsdfsafdsfasf");
            _configMock
                .Setup(x => x["Jwt:ExpiryInMinutes"])
                .Returns("60");
            _tokenService = new JwtTokenService(_configMock.Object);
        }

        [Test]
        public void GetToken_ClaimsAreNull_WeGetToken()
        {
            var token = _tokenService.GetToken(null);
            Assert.IsTrue(token != null);
        }

        [Test]
        public void GetToken_ClaimsAreNotNullButEmpty_WeGetToken()
        {
            var claims = new List<Claim>();
            var token = _tokenService.GetToken(claims);
            Assert.IsTrue(token != null);
        }

        [Test]
        public void GetToken_ClaimsAreNotNull_WeGetToken()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, Roles.User.ToString())
            };
            var token = _tokenService.GetToken(claims);
            Assert.IsTrue(token != null);
        }
    }
    
}

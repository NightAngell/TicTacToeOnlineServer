using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicTacToeServer.Controllers;
using TicTacToeServer.DTO;
using TicTacToeServer.Enums;
using TicTacToeServer.Models;
using TicTacToeServer.Services;

namespace TicTacToeServerTests.Controllers
{
    [TestFixture]
    class AuthControllerTests
    {
        AuthController _authController;
        Mock<UserManager<AppUser>> _userManagerMock;
        Mock<IConfiguration> _configMock;
        Mock<IGuidService> _guidServiceMock;

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = _getUserManagerMock();
            _configMock = new Mock<IConfiguration>();
            _guidServiceMock = new Mock<IGuidService>();
        }

        //Register
        [Test]
        public async Task Register_UserDataValid_UserRegistratedWithUserRole()
        {
            _setupMocksForRegister_UserDataValid_UserRegistratedWithUserRole();
            _initAuthController();

            var actionResult = await _authController.Register(_getValidRegisterDto());

            Assert.IsTrue(actionResult is OkResult);
            _userManagerMock.Verify(
                x => x.AddToRoleAsync(It.IsAny<AppUser>(), Roles.User.ToString()),
                Times.Once
            );
        }
        private void _setupMocksForRegister_UserDataValid_UserRegistratedWithUserRole()
        {
            Guid guid = Guid.NewGuid();
            var validUser = _getValidRegisterDto();
            var validAppUser = new AppUser
            {
                Email = validUser.Email,
                UserName = validUser.Email,
                SecurityStamp = guid.ToString()
            };

            _guidServiceMock.Setup(x => x.NewGuid()).Returns(guid);

            _userManagerMock
                .Setup(
                    x => x.CreateAsync(
                        It.Is<AppUser>(
                            u => u.Email == validUser.Email
                                 && u.UserName == validUser.Email
                                 && u.SecurityStamp == guid.ToString()
                        ),
                        validUser.Password
                    )
                )
                .Returns(Task.FromResult(IdentityResult.Success));
        }

        [Test]
        public async Task Register_UserDataInvalid_UserGetBadRequestWithErrorList()
        {
            _setupMocksForRegister_UserDataInvalid_UserGetBadRequestWithErrorList();
            _initAuthController();

            var actionResult = await _authController.Register(new RegisterDto() {
                Email = "lorem",
                Password = "ipsum"
            });
            var badRequestObjectResult = actionResult as BadRequestObjectResult;
            var requestValue = badRequestObjectResult.Value as List<IdentityError>;

            Assert.IsTrue(badRequestObjectResult != null);
            Assert.IsTrue(requestValue.Count > 0);
        }
        private void _setupMocksForRegister_UserDataInvalid_UserGetBadRequestWithErrorList()
        {
            Guid guid = Guid.NewGuid();
            _guidServiceMock.Setup(x => x.NewGuid()).Returns(guid);

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));
        }

        //Login

        //Helpers
        private void _initAuthController()
        {
            _authController = new AuthController(
                _userManagerMock.Object,
                _configMock.Object,
                _guidServiceMock.Object
            );
        }
        private RegisterDto _getValidRegisterDto()
        {
            return new RegisterDto{
                Email = "Valid@User.Data",
                Password = "qwertyu123"
            };
        }
        private Mock<UserManager<AppUser>> _getUserManagerMock()
        {
            var storeObject = (new Mock<IUserStore<AppUser>>()).Object;
            return new Mock<UserManager<AppUser>>(
                storeObject,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
        }
    }
}

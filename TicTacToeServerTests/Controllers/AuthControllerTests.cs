using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        Mock<IJwtTokenService> _tokenServiceMock;

        readonly LoginDto _validLoginDto = new LoginDto
        {
            Email = "Valid@User.Data",
            Password = "qwertyu123"
        };
        readonly RegisterDto _validRegisterDto = new RegisterDto
        {
            Email = "Valid@User.Data",
            Password = "qwertyu123"
        };
        readonly RegisterDto _invalidRegisterDto = new RegisterDto
        {
            Email = "Lorem",
            Password = "Ipsum"
        };

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = _getUserManagerMock();
            _configMock = new Mock<IConfiguration>();
            _guidServiceMock = new Mock<IGuidService>();
            _tokenServiceMock = new Mock<IJwtTokenService>();
        }

        //Register
        [Test]
        public async Task Register_UserDataValid_UserRegistratedWithUserRole()
        {
            _setupMocksForRegister_UserDataValid_UserRegistratedWithUserRole();
            _initAuthController();

            var actionResult = await _authController.Register(_validRegisterDto);

            Assert.IsTrue(actionResult is OkResult);
            _userManagerMock.Verify(
                x => x.AddToRoleAsync(It.IsAny<AppUser>(), Roles.User.ToString()),
                Times.Once
            );
        }
        private void _setupMocksForRegister_UserDataValid_UserRegistratedWithUserRole()
        {
            Guid guid = Guid.NewGuid();
            var validAppUser = new AppUser
            {
                Email = _validRegisterDto.Email,
                UserName = _validRegisterDto.Email,
                SecurityStamp = guid.ToString()
            };

            _guidServiceMock.Setup(x => x.NewGuid()).Returns(guid);

            _userManagerMock
                .Setup(
                    x => x.CreateAsync(
                        It.Is<AppUser>(
                            u => u.Email == _validRegisterDto.Email
                                 && u.UserName == _validRegisterDto.Email
                                 && u.SecurityStamp == guid.ToString()
                        ),
                        _validRegisterDto.Password
                    )
                )
                .Returns(Task.FromResult(IdentityResult.Success));
        }

        [Test]
        public async Task Register_UserDataInvalid_UserGetBadRequestWithErrorList()
        {
            _setupMocksForRegister_UserDataInvalid_UserGetBadRequestWithErrorList();
            _initAuthController();

            var actionResult = await _authController.Register(_invalidRegisterDto);
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
                .Setup(x => x.CreateAsync(
                    It.Is<AppUser>(
                        a => a.Email == _invalidRegisterDto.Email && a.UserName == _invalidRegisterDto.Email
                    ),
                    _invalidRegisterDto.Password
                 ))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));
        }

        //Login
        [Test]
        public async Task Login_LoginDataValid_ReturnTokenWithExpirtion()
        {
            _setupForLogin_LoginDataValid_ReturnTokenWithExpirtion();
            _initAuthController();

            var actionResult = await _authController.Login(_validLoginDto);
            var okObjectResult = actionResult as OkObjectResult;
            var tokenWithExpiration = okObjectResult.Value as TokenWithExpirationDto;

            Assert.IsTrue(okObjectResult != null);
            Assert.IsTrue(tokenWithExpiration.Token != null);
            Assert.IsTrue(tokenWithExpiration.Expiration != null);
        }
        private void _setupForLogin_LoginDataValid_ReturnTokenWithExpirtion()
        {
            var appUser = new AppUser() { Email = "w@w.w", UserName = "w@w.w" };
            _userManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(appUser);
            _userManagerMock
                .Setup(
                    x => x.CheckPasswordAsync(
                        It.Is<AppUser>(
                            d => d.Email == appUser.Email
                            && d.UserName == appUser.UserName
                        ),
                        _validLoginDto.Password
                   )
                )
                .ReturnsAsync(true);
            _tokenServiceMock
                .Setup(x => x.GetToken(It.IsAny<List<Claim>>()))
                .Returns(new JwtSecurityToken());
        }

        [Test]
        public async Task Login_LoginEmailInvalid_ReturnUnauthorizedAccess()
        {
            _initAuthController();

            var actionResult = await _authController.Login(new LoginDto {
                Email = "invalidEmail.pl",
                Password = "qwerty123"
            });

            Assert.IsTrue(actionResult is UnauthorizedResult);
        }

        [Test]
        public async Task Login_LoginPasswordInvalid_ReturnUnauthorizedAccess()
        {
            _userManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser() { });
             _userManagerMock
                .Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), ""))
                .ReturnsAsync(false);
            _initAuthController();

            var actionResult = await _authController.Login(new LoginDto
            {
                Email = "valid@Email.com",
                Password = ""
            });

            Assert.IsTrue(actionResult is UnauthorizedResult);
        }

        //RefreshToken
        [Test]
        public async Task RefreshToken_TokenRefreshed()
        {
            _tokenServiceMock
                .Setup(x => x.GetToken(It.IsAny<List<Claim>>()))
                .Returns(new JwtSecurityToken());
            _initAuthController();

            var actionResult = await _authController.RefreshToken();
            var okObjectResult = actionResult as OkObjectResult;
            var tokenWithExpiration = okObjectResult.Value as TokenWithExpirationDto;

            Assert.IsTrue(okObjectResult != null);
            Assert.IsTrue(tokenWithExpiration.Token != null);
            Assert.IsTrue(tokenWithExpiration.Expiration != null);
        }

        //Helpers
        private void _initAuthController()
        {
            _authController = new AuthController(
                _userManagerMock.Object,
                _configMock.Object,
                _guidServiceMock.Object,
                _tokenServiceMock.Object
            );
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

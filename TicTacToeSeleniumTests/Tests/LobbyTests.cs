using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicTacToeSeleniumTests.PageObjects;

namespace TicTacToeSeleniumTests.Tests
{
    [TestFixture]
    class LobbyTests : SeleniumWithWorkingAuthBase
    {
        [Test]
        public void UserClickMenuButton_UserIsRedirectedToMenu()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                LogIn(driver);

                var lobbyPage = new LobbyPage(driver);
                lobbyPage.Navigate();

                lobbyPage.MenuButton.Click();

                _verifyUserIsRedirected("", driver);

                LogOut(driver);
            }
        }

        [Test]
        public void UserHaveNickAndPasswordThenClickHostButtonThenClickAbortRoomButton_UserCanSeeRoomIdAndNickAndPasswordThenRoomIsAborted()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                LogIn(driver);

                var lobbyPage = new LobbyPage(driver);
                lobbyPage.Navigate();

                lobbyPage.PlayerNickInput.SendKeys("nick");
                lobbyPage.PasswordInput.SendKeys("password");
                lobbyPage.HostButton.Click();

                _waitForElement(driver, By.ClassName(LobbyPage.modalContentClassName));

                Assert.Greater(lobbyPage.ModalHostedRoomId.Text.Length, 0);
                Assert.AreEqual("nick", lobbyPage.ModalHostNick.Text);
                Assert.AreEqual("password", lobbyPage.ModalHostedRoomPassword.Text);

                lobbyPage.ModalAbortButton.Click();

                _waitForElement(driver, By.ClassName("logout"));
                LogOut(driver);
            }
        }

        [Test]
        public void WeHaveHostAndGuestUsersWhichTryStartGame_UsersRedirectedToMultiplayerGame()
        {
            using (var hostDriver = _getNewInstanceOfRequiredDriver())
            using (var guestDriver = _getNewInstanceOfRequiredDriver())
            {
                LogIn(hostDriver);
                LogIn(guestDriver);

                var hostLobbyPage = new LobbyPage(hostDriver);
                hostLobbyPage.Navigate();
                var guestLobbyPage = new LobbyPage(guestDriver);
                guestLobbyPage.Navigate();

                hostLobbyPage.HostButton.Click();
                _waitForElement(hostDriver, By.ClassName(LobbyPage.modalContentClassName));

                guestLobbyPage.RefreshButton.Click();
                _waitForElement(guestDriver, By.CssSelector(LobbyPage.firstRoomInLobbySelector));
                guestLobbyPage.FirstRoom.Click();

                _verifyUserIsRedirected("multiplayerGame", hostDriver);
                _verifyUserIsRedirected("multiplayerGame", guestDriver);

                hostLobbyPage.Navigate();
                LogOut(hostDriver);
                guestLobbyPage.Navigate();
                LogOut(guestDriver);
            }
        }
    }
}

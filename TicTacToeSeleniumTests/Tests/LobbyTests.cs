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
                LobbyPage hostLobbyPage, guestLobbyPage;
                _startGameUsingLobby(hostDriver, guestDriver, out hostLobbyPage, out guestLobbyPage);

                _verifyUserIsRedirected("multiplayerGame", hostDriver);
                _verifyUserIsRedirected("multiplayerGame", guestDriver);

                _logoutBothPlayers(hostDriver, guestDriver, hostLobbyPage, guestLobbyPage);
            }
        }

        [Test]
        public void GuestClickOnRoomButRoomWasAbortedByHostLittleEarlier_GuestSeeRoomNotExistError()
        {
            using (var hostDriver = _getNewInstanceOfRequiredDriver())
            using (var guestDriver = _getNewInstanceOfRequiredDriver())
            {
                LobbyPage hostLobbyPage, guestLobbyPage;
                _loginBothPlayersAndNavigateThemToLobby(hostDriver, guestDriver, out hostLobbyPage, out guestLobbyPage);

                hostLobbyPage.HostButton.Click();
                _waitForElement(hostDriver, By.ClassName(LobbyPage.modalContentClassName));
                guestLobbyPage.RefreshButton.Click();
                _waitForElement(guestDriver, By.CssSelector(LobbyPage.firstRoomInLobbySelector));
                hostLobbyPage.ModalAbortButton.Click();
                guestLobbyPage.FirstRoom.Click();

                _waitForElement(guestDriver, By.CssSelector(LobbyPage.infoModalInfoDivSelector));
                Assert.IsTrue(guestLobbyPage.InfoModalInfoDiv.Text.Contains("Room not exist"));

                _logoutBothPlayers(hostDriver, guestDriver, hostLobbyPage, guestLobbyPage);
            }
        }

        [Test]
        [TestCase("( ͡° ʖ̯ ͡°)")]
        [TestCase("")]
        public void HostCreateRoomWithPasswordGuestTryGetAccessWithWrongPasswod_GuestSeeWrongPasswordError(string guestPassword)
        {
            using (var hostDriver = _getNewInstanceOfRequiredDriver())
            using (var guestDriver = _getNewInstanceOfRequiredDriver())
            {
                LobbyPage hostLobbyPage, guestLobbyPage;
                _loginBothPlayersAndNavigateThemToLobby(hostDriver, guestDriver, out hostLobbyPage, out guestLobbyPage);

                hostLobbyPage.PasswordInput.SendKeys("TruePassword");
                hostLobbyPage.HostButton.Click();
                _waitForElement(hostDriver, By.ClassName(LobbyPage.modalContentClassName));
                guestLobbyPage.RefreshButton.Click();
                _waitForElement(guestDriver, By.CssSelector(LobbyPage.firstRoomInLobbySelector));
                guestLobbyPage.PasswordInput.SendKeys(guestPassword);
                guestLobbyPage.FirstRoom.Click();
                _waitForElement(guestDriver, By.CssSelector(LobbyPage.infoModalInfoDivSelector));

                Assert.IsTrue(guestLobbyPage.InfoModalInfoDiv.Text.Contains("Wrong password"));

                hostLobbyPage.ModalAbortButton.Click();
                _logoutBothPlayers(hostDriver, guestDriver, hostLobbyPage, guestLobbyPage);
            }
        }

        private void _loginBothPlayersAndNavigateThemToLobby(RemoteWebDriver hostDriver, RemoteWebDriver guestDriver, out LobbyPage hostLobbyPage, out LobbyPage guestLobbyPage)
        {
            LogIn(hostDriver);
            LogIn(guestDriver);
            hostLobbyPage = new LobbyPage(hostDriver);
            hostLobbyPage.Navigate();
            guestLobbyPage = new LobbyPage(guestDriver);
            guestLobbyPage.Navigate();
        }

        private void _logoutBothPlayers(RemoteWebDriver hostDriver, RemoteWebDriver guestDriver, LobbyPage hostLobbyPage, LobbyPage guestLobbyPage)
        {
            hostLobbyPage.Navigate();
            LogOut(hostDriver);
            guestLobbyPage.Navigate();
            LogOut(guestDriver);
        }

        private void _startGameUsingLobby(RemoteWebDriver hostDriver, RemoteWebDriver guestDriver, out LobbyPage hostLobbyPage, out LobbyPage guestLobbyPage)
        {
            hostLobbyPage = new LobbyPage(hostDriver);
            guestLobbyPage = new LobbyPage(guestDriver);
            hostLobbyPage.Navigate();
            guestLobbyPage.Navigate();

            hostLobbyPage.HostButton.Click();
            _waitForElement(hostDriver, By.ClassName(LobbyPage.modalContentClassName));

            guestLobbyPage.RefreshButton.Click();
            _waitForElement(guestDriver, By.CssSelector(LobbyPage.firstRoomInLobbySelector));
            guestLobbyPage.FirstRoom.Click();
        }
    }
}

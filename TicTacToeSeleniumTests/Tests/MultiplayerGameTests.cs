using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeSeleniumTests.PageObjects;

namespace TicTacToeSeleniumTests.Tests
{
    [TestFixture]
    class MultiplayerGameTests : SeleniumWithWorkingAuthBase
    {
        [Test]
        public void UserDirectlyNavigateToOnlineGame_UserSeeInfiniteLoadingBar()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                LogIn(driver);

                var page = new MultiplayerGamePage(driver);
                page.Navigate();

                _verifyIfElementExist(driver, By.ClassName("lds-roller"));

                //We need navigate because logout button must be clickable
                new LobbyPage(driver).Navigate();
                LogOut(driver);
            }
        }

        [Test]
        public void HostAndGuestWantPlayBetweenThem_UsersRedirectedToPageWithGame()
        {
            using (var hostDriver = _getNewInstanceOfRequiredDriver())
            using (var guestDriver = _getNewInstanceOfRequiredDriver())
            {
                LobbyPage hostLobbyPage, guestLobbyPage;
                MultiplayerGamePage hostGame, guestGame;
                _startGameUsingLobby(
                    hostDriver, 
                    guestDriver, 
                    out hostLobbyPage, 
                    out guestLobbyPage,
                    out hostGame,
                    out guestGame
                 );

                _verifyUserIsRedirected("multiplayerGame", hostDriver);
                _verifyUserIsRedirected("multiplayerGame", guestDriver);

                _logoutBothPlayers(hostLobbyPage, guestLobbyPage);
            }
        }

        [Test]
        public void HostWinGame_HostSeeCongratulationsModalAndGuestSeeOuhModal()
        {
            using (var hostDriver = _getNewInstanceOfRequiredDriver())
            using (var guestDriver = _getNewInstanceOfRequiredDriver())
            {
                LobbyPage hostLobbyPage, guestLobbyPage;
                MultiplayerGamePage hostGame, guestGame;
                _startGameUsingLobby(
                    hostDriver, 
                    guestDriver, 
                    out hostLobbyPage, 
                    out guestLobbyPage,
                    out hostGame,
                    out guestGame
                 );

                hostGame.MiddleButton.Click();
                _waitForTextInElement(guestDriver, guestGame.MiddleButton, "x");

                guestGame.TopButton.Click();
                _waitForTextInElement(hostDriver, guestGame.TopButton, "o");

                hostGame.MiddleLeftButton.Click();
                _waitForTextInElement(guestDriver, guestGame.MiddleLeftButton, "x");

                guestGame.TopLeftButton.Click();
                _waitForTextInElement(hostDriver, guestGame.TopLeftButton, "o");

                hostGame.MiddleRightButton.Click();
                _waitForTextInElement(guestDriver, guestGame.MiddleRightButton, "x");

                _verifyIfElementExist(hostDriver, By.ClassName("modalOkButton"));
                _verifyIfElementExist(guestDriver, By.ClassName("modalOkButton"));

                _logoutBothPlayers(hostLobbyPage, guestLobbyPage);
            }
        }

        private void _logoutBothPlayers(LobbyPage hostLobbyPage, LobbyPage guestLobbyPage)
        {
            hostLobbyPage.Navigate();
            LogOut(hostLobbyPage.Driver);
            guestLobbyPage.Navigate();
            LogOut(guestLobbyPage.Driver);
        }

        /// <summary>
        ///  Should redirect multiplayerGame.
        ///  We dont want navigate using MultiplayerGamePagen
        ///  because game itself cannot create valid match
        /// </summary>
        private void _startGameUsingLobby(
            RemoteWebDriver hostDriver, 
            RemoteWebDriver guestDriver, 
            out LobbyPage hostLobbyPage, 
            out LobbyPage guestLobbyPage,
            out MultiplayerGamePage hostGamePage ,
            out MultiplayerGamePage guestGamePage )
        {
            LogIn(hostDriver);
            LogIn(guestDriver);
            hostLobbyPage = new LobbyPage(hostDriver);
            guestLobbyPage = new LobbyPage(guestDriver);
            hostLobbyPage.Navigate();
            guestLobbyPage.Navigate();

            hostLobbyPage.HostButton.Click();
            _waitForElement(hostDriver, By.ClassName(LobbyPage.modalContentClassName));

            guestLobbyPage.RefreshButton.Click();
            _waitForElement(guestDriver, By.CssSelector(LobbyPage.firstRoomInLobbySelector));
            guestLobbyPage.FirstRoom.Click();

            hostGamePage = new MultiplayerGamePage(hostDriver);
            guestGamePage = new MultiplayerGamePage(guestDriver);

            _waitUntilUserIsRedirected("multiplayerGame", hostDriver);
            _waitUntilUserIsRedirected("multiplayerGame", guestDriver);
        }
    }
}

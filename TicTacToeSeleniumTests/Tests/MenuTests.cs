using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeSeleniumTests.PageObjects;

namespace TicTacToeSeleniumTests.Tests
{
    [TestFixture]
    class MenuTests : SeleniumWithWorkingAuthBase
    {
        [Test]
        public void UserClickPlayButton_UserIsRedirectedToSingleplayerGamePage()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                LogIn(driver);

                var menuPage = new MenuPage(driver);
                menuPage.Navigate();
                menuPage.SingleplayerGameButton.Click();

                _verifyUserIsRedirected("ticTacToe", driver);

                LogOut(driver);
            }
        }

        [Test]
        public void UserClickPlayButton_UserIsRedirectedToMutliplayerLobby()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                LogIn(driver);

                var menuPage = new MenuPage(driver);
                menuPage.Navigate();
                menuPage.MultiplayerLobbyButton.Click();

                _verifyUserIsRedirected("lobby", driver);

                LogOut(driver);
            }
        }
    }
}

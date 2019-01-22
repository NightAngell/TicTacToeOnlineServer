using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TicTacToeSeleniumTests.PageObjects;

namespace TicTacToeSeleniumTests.Tests
{
    [TestFixture]
    class TicTacToeSingleplayerTests : SeleniumWithWorkingAuthBase
    {
        [Test]
        public void UserPlayWithBot_UserLeaveGameByClickOkButtonAfterGameEnded()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                LogIn(driver);
                var page = new TicTacToeSingleplayerPage(driver);
                page.Navigate();
                int limit = 9;
                var buttons = page.GetListOfGameButtons();
                while(page.ModalOkButton == null && limit > 0)
                {
                    for(int i = 0; i < buttons.Count; i++)
                    {
                        _clickIfEmptyAndModalOkButtonStillNotExist(buttons[i], page);
                    }
                    limit--;
                }

                page.ModalOkButton.Click();
                _verifyUserIsRedirected("", driver);
                LogOut(driver);
            }
        }

        /// <summary>
        ///  It require short delay, because browser need time to refresh
        /// </summary>
        private void _clickIfEmptyAndModalOkButtonStillNotExist(
            IWebElement button, 
            TicTacToeSingleplayerPage page,
            int timeForPageStateRefreshInMs = 500
        )
        {
            Thread.Sleep(timeForPageStateRefreshInMs);
            if(button.Text == "" && page.ModalOkButton == null)
            {
                button.Click(); 
            }
        }
    }
}

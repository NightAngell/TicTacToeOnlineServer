using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumExtras.PageObjects;

namespace TicTacToeSeleniumTests.PageObjects
{
    class MenuPage : PageObjectBase
    {
        public MenuPage(RemoteWebDriver driver) 
            : base(driver, ""){}

        [FindsBy(How = How.CssSelector, Using = "li[routerlink='ticTacToe']")]
        public IWebElement SingleplayerGameButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = "li[routerlink='lobby']")]
        public IWebElement MultiplayerLobbyButton { get; set; }
    }
}

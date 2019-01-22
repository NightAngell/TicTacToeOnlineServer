using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumExtras.PageObjects;

namespace TicTacToeSeleniumTests.PageObjects
{
    class MultiplayerGamePage : PageObjectBase
    {
        public const string TopLeftButtonSelector = "app-game > div > div:nth-child(1) > div:nth-child(1)";
        public const string TopButtonSelector = "aapp-game > div > div:nth-child(1) > div:nth-child(2)";
        public const string TopRightButtonSelector = "app-game > div > div:nth-child(1) > div:nth-child(3)";

        public const string MiddleLeftButtonSelector = "app-game > div > div:nth-child(2) > div:nth-child(1)";
        public const string MiddleButtonSelector = "aapp-game > div > div:nth-child(2) > div:nth-child(2)";
        public const string MiddleRightButtonSelector = "app-game > div > div:nth-child(2) > div:nth-child(3)";

        public const string BottomLeftButtonSelector = "app-game > div > div:nth-child(3) > div:nth-child(1)";
        public const string BotoomButtonSelector = "app-game > div > div:nth-child(3) > div:nth-child(2)";
        public const string BottomRightButtonSelector = "app-game > div > div:nth-child(3) > div:nth-child(3)";

        public MultiplayerGamePage(RemoteWebDriver driver) 
            : base(driver, "multiplayerGame"){}

        [FindsBy(How = How.CssSelector, Using = TopLeftButtonSelector)]
        public IWebElement TopLeftButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = TopButtonSelector)]
        public IWebElement TopButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = TopRightButtonSelector)]
        public IWebElement TopRightButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = MiddleLeftButtonSelector)]
        public IWebElement MiddleLeftButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = MiddleButtonSelector)]
        public IWebElement MiddleButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = MiddleRightButtonSelector)]
        public IWebElement MiddleRightButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = BottomLeftButtonSelector)]
        public IWebElement BottomLeftButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = BotoomButtonSelector)]
        public IWebElement BotoomButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = BottomRightButtonSelector)]
        public IWebElement BottomRightButton { get; set; }

        public IWebElement ModalOkButton
        {
            get
            {
                IWebElement button;
                try
                {
                    button = _driver.FindElementByClassName("okButton");
                }
                catch (NoSuchElementException e)
                {
                    button = null;
                }

                return button;
            }
        }
    }
}

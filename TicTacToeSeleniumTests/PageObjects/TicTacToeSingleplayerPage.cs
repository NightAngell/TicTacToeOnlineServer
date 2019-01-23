using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumExtras.PageObjects;

namespace TicTacToeSeleniumTests.PageObjects
{
    class TicTacToeSingleplayerPage : PageObjectBase
    {
        public const string TopLeftButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(1) > div:nth-child(1)";
        public const string TopButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(1) > div:nth-child(2)";
        public const string TopRightButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(1) > div:nth-child(3)";

        public const string MiddleLeftButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(2) > div:nth-child(1)";
        public const string MiddleButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(2) > div:nth-child(2)";
        public const string MiddleRightButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(2) > div:nth-child(3)";

        public const string BottomLeftButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(3) > div:nth-child(1)";
        public const string BotoomButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(3) > div:nth-child(2)";
        public const string BottomRightButtonSelector = "app-tic-tac-toe-singleplayer > div > div:nth-child(3) > div:nth-child(3)";

        public TicTacToeSingleplayerPage(RemoteWebDriver driver) 
            : base(driver, "ticTacToe"){}


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

        public List<IWebElement> GetListOfGameButtons()
        {
            return new List<IWebElement>
            {
                TopLeftButton,
                TopButton,
                TopRightButton,

                MiddleLeftButton,
                MiddleButton,
                MiddleRightButton,

                BottomLeftButton,
                BotoomButton,
                BottomRightButton
            };
        }

        public IWebElement ModalOkButton
        {
            get
            {
                IWebElement button;
                try
                {
                    button = Driver.FindElementByClassName("okButton");
                }
                catch(NoSuchElementException e)
                {
                    button = null;
                }

                return button;
            }
        }
    }
}

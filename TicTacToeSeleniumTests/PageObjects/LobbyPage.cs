using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumExtras.PageObjects;

namespace TicTacToeSeleniumTests.PageObjects
{
    class LobbyPage : PageObjectBase
    {
        public const string modalContentClassName = "modalContent";
        public const string infoModalTagName = "app-info-modal";
        public const string infoModalInfoDivSelector = "app-info-modal .modalInfo";
        public const string infoModalButtonDivButtonSelector = "app-info-modal .modalOkButton";

        /// <summary>
        /// This element exist only if room is hosted in lobby
        /// </summary>
        public const string firstRoomInLobbySelector = "app-lobby > div > div.bottomLobby.flex-col-9.flex-container.flex-row > div.listOfRooms.flex-col-9 > div";

        public LobbyPage(RemoteWebDriver driver) 
            : base(driver, "lobby")
        {
        }

        [FindsBy(How = How.ClassName, Using = "playerNickInput")]
        public IWebElement PlayerNickInput { get; set; }

        [FindsBy(How = How.ClassName, Using = "paswordInput")]
        public IWebElement PasswordInput { get; set; }

        [FindsBy(How = How.ClassName, Using = "hostButton")]
        public IWebElement HostButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "refreshButton")]
        public IWebElement RefreshButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "menuButton")]
        public IWebElement MenuButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "listOfRooms")]
        public IWebElement ListOfRooms { get; set; }

        [FindsBy(How = How.ClassName, Using = modalContentClassName)]
        public IWebElement ModalContent { get; set; }

        [FindsBy(How = How.ClassName, Using = "abortButton")]
        public IWebElement ModalAbortButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "modalHostedRoomId")]
        public IWebElement ModalHostedRoomId { get; set; }

        [FindsBy(How = How.ClassName, Using = "modalHostNick")]
        public IWebElement ModalHostNick { get; set; }

        [FindsBy(How = How.ClassName, Using = "modalHostedRoomPassword")]
        public IWebElement ModalHostedRoomPassword { get; set; }

        /// <summary>
        /// This element exist only if room is hosted in lobby
        /// </summary>
        [FindsBy(How = How.CssSelector, Using = firstRoomInLobbySelector)]
        public IWebElement FirstRoom { get; set; }

        [FindsBy(How = How.TagName, Using = infoModalTagName)]
        public IWebElement InfoModal { get; set; }

        [FindsBy(How = How.CssSelector, Using = infoModalInfoDivSelector)]
        public IWebElement InfoModalInfoDiv { get; set; }

        [FindsBy(How = How.CssSelector, Using = infoModalButtonDivButtonSelector)]
        public IWebElement InfoModalOkButton { get; set; }
    }
}

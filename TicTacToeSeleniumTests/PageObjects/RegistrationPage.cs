using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.PageObjects;

namespace TicTacToeSeleniumTests.PageObjects
{
    class RegistrationPage : PageObjectBase
    {
        public RegistrationPage(RemoteWebDriver driver)
            : base(driver, "registration"){}

        [FindsBy(How = How.CssSelector, Using = "")]
        public IWebElement LoginInput { get; set; }
    }
}

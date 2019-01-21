using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToeSeleniumTests.PageObjects
{

    class LoginPage : PageObjectBase
    {
        public const string loginCssSelector = "app-loginandregistration > form > input[name='email']";
        public const string passwordCssSelector = "app-loginandregistration > form > input[name='password']";
        public const string submitButtonCssSelector = "app-loginandregistration > form > input[type='submit']";
        public const string emailInvalidInforDivSelector = "div[emailInvalidInfo]";
        public const string passwordInfalidInfoForDivSelector = "div[passwordInvalidInfo]";
        public const string wrongLoginOrPasswordInfoDivSelector = "div[wrongLoginOrPasswordInfo]";
        public const string redirectLinkDivSelector = "div[routerLink='/registration']";

        public LoginPage(RemoteWebDriver driver) : base (driver, "login")
        {
        }

        [FindsBy(How = How.CssSelector, Using = loginCssSelector)]
        public IWebElement LoginInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = passwordCssSelector)]
        public IWebElement PasswordInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = submitButtonCssSelector)]
        public IWebElement SubmitButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = emailInvalidInforDivSelector)]
        public IWebElement EmailInvalidInfoDiv { get; set; }

        [FindsBy(How = How.CssSelector, Using = passwordInfalidInfoForDivSelector)]
        public IWebElement PasswordInvalidInfoDiv { get; set; }

        [FindsBy(How = How.CssSelector, Using = wrongLoginOrPasswordInfoDivSelector)]
        public IWebElement WrongLoginOrPasswordDiv { get; set; }

        [FindsBy(How = How.CssSelector, Using = redirectLinkDivSelector)]
        public IWebElement RedirectToRegistrationDiv { get; set; }
    }
}

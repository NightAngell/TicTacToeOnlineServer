using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToeSeleniumTests.PageObjects
{

    class LoginPage
    {
        protected readonly IWebDriver _driver;
        protected const string loginCssSelector = "app-loginandregistration > form > input[name='email']";
        protected const string passwordCssSelector = "app-loginandregistration > form > input[name='password']";
        protected const string submitButtonCssSelector = "app-loginandregistration > form > input[type='submit']";

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            PageFactory.InitElements(_driver, this);
        }

        [FindsBy(How = How.CssSelector, Using = loginCssSelector)]
        public IWebElement LoginInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = passwordCssSelector)]
        public IWebElement PasswordInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = submitButtonCssSelector)]
        public IWebElement SubmitButton { get; set; }

        public void Navigate()
        {
            _driver
                .Navigate()
                .GoToUrl("http://localhost:4200/login");
        }
    }
}

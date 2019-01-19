using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeSeleniumTests.PageObjects;

namespace TicTacToeSeleniumTests.Auth
{
    [TestFixture]
    class LoginTests
    {
        FirefoxDriver _firefoxDriver = new FirefoxDriver(Environment.CurrentDirectory);

        [Test]
        public void WeHaveValidLoginAndPassword_UserLoggedIn()
        {
            var loginPage = new LoginPage(_firefoxDriver);
            loginPage.Navigate();
            loginPage.LoginInput.SendKeys("sobta24@wp.pl");
            loginPage.PasswordInput.SendKeys("qwerty123");
            loginPage.SubmitButton.Submit();

            //ClearAfterTest
            _firefoxDriver.FindElementByClassName("logout").Click();
        }

        [TearDown]
        public void TearDown()
        {
           _firefoxDriver.Close();
        }
    }
}

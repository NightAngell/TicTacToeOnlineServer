using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeSeleniumTests.PageObjects;

namespace TicTacToeSeleniumTests
{
    abstract class SeleniumWithWorkingAuthBase : SeleniumTestsBase
    {
        protected void LogIn(RemoteWebDriver driver)
        {
            var loginPage = new LoginPage(driver);
            loginPage.Navigate();
            loginPage.LoginInput.SendKeys(LoginAndPassword.Login);
            loginPage.PasswordInput.SendKeys(LoginAndPassword.Password);
            loginPage.SubmitButton.Submit();

            _waitForElement(driver, By.CssSelector(".logout"));
        }

        protected void LogOut(RemoteWebDriver driver)
        {
            driver.FindElementByClassName("logout").Click();
        }
    }
}

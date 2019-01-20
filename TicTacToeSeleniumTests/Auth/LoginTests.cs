using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
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
        private RemoteWebDriver _getRequiredDriver()
        {
            return new FirefoxDriver(Environment.CurrentDirectory);
        }

        [Test]
        public void WeHaveValidLoginAndPassword_UserLoggedIn()
        {
            using (var driver = _getRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.LoginInput.SendKeys(LoginAndPassword.Login);
                loginPage.PasswordInput.SendKeys(LoginAndPassword.Password);
                loginPage.SubmitButton.Submit();

                //Check it logout button exist, If yes, user is logged in
                new WebDriverWait(driver, TimeSpan.FromSeconds(40))
                    .Until(ExpectedConditions.ElementExists(By.CssSelector(".logout")));

                //Clear after test
                driver.FindElementByClassName("logout").Click();
            }
        }

        [Test]
        public void WeHaveInvalidLogin_UserSeeEmailInvalidInfoDiv()
        {
            using (var driver = _getRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.LoginInput.SendKeys("invalidemail.com");
                loginPage.PasswordInput.Click();

                Assert.IsTrue(loginPage.EmailInvalidInfoDiv.Displayed);
            }
        }

        [Test]
        public void WeHaveTooShortPassword_UserSeePasswordInvalidInfoDiv()
        {
            using (var driver = _getRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.PasswordInput.Click();
                loginPage.LoginInput.Click();

                Assert.IsTrue(loginPage.PasswordInvalidInfoDiv.Displayed);
            }
        }

        [Test]
        public void LoginIsValidEmailButPasswordIsInvalidInAccounContext_UserSeeWrongLoginOrPasswordDiv()
        {
            using (var driver = _getRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.PasswordInput.SendKeys($"{LoginAndPassword.Password}a");
                loginPage.LoginInput.SendKeys(LoginAndPassword.Login);
                loginPage.SubmitButton.Submit();

                new WebDriverWait(driver, TimeSpan.FromSeconds(40))
                    .Until(ExpectedConditions.ElementExists(
                        By.CssSelector(LoginPage.wrongLoginOrPasswordInfoDivSelector))
                    );

                Assert.IsTrue(loginPage.WrongLoginOrPasswordDiv.Displayed);
            } 
        }

        [Test]
        public void UserClickToRedirectToRegistrationButton_UserRedirectedToRegistrationPage()
        {
            using (var driver = _getRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.RedirectToRegistrationDiv.Click();

                //Check if user is redirected to registration page
                new WebDriverWait(driver, TimeSpan.FromSeconds(40))
                    .Until(ExpectedConditions.UrlContains("registration"));
            }
        }
    }
}

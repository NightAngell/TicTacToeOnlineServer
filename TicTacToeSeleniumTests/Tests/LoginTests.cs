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

namespace TicTacToeSeleniumTests.Tests
{
    [TestFixture]
    class LoginTests : SeleniumTestsBase
    {
        [Test]
        public void WeHaveValidLoginAndPassword_UserLoggedIn()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.LoginInput.SendKeys(LoginAndPassword.Login);
                loginPage.PasswordInput.SendKeys(LoginAndPassword.Password);
                loginPage.SubmitButton.Submit();

                //Check it logout button exist, If yes, user is logged in
                _verifyIfElementExistAfter(
                    TimeSpan.FromSeconds(40), 
                    driver, 
                    By.CssSelector(".logout"));

                //Clear after test
                driver.FindElementByClassName("logout").Click();
            }
        }

        [Test]
        public void WeHaveInvalidLogin_UserSeeEmailInvalidInfoDivAndSubmitButtonIsDisabled()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.LoginInput.SendKeys("invalidemail.com");
                loginPage.PasswordInput.Click();

                Assert.IsTrue(loginPage.EmailInvalidInfoDiv.Displayed);
                Assert.IsFalse(loginPage.SubmitButton.Enabled);
            }
        }

        [Test]
        public void WeHaveTooShortPassword_UserSeePasswordInvalidInfoDivAndSubmitButtonIsDisabled()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.PasswordInput.Click();
                loginPage.LoginInput.Click();

                Assert.IsTrue(loginPage.PasswordInvalidInfoDiv.Displayed);
                Assert.IsFalse(loginPage.SubmitButton.Enabled);
            }
        }

        [Test]
        public void LoginIsValidEmailButPasswordIsInvalidInAccounContext_UserSeeWrongLoginOrPasswordDiv()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.PasswordInput.SendKeys($"{LoginAndPassword.Password}a");
                loginPage.LoginInput.SendKeys(LoginAndPassword.Login);
                loginPage.SubmitButton.Submit();

                _waitForElement(
                    driver,
                    By.CssSelector(LoginPage.wrongLoginOrPasswordInfoDivSelector)
                 );

                Assert.IsTrue(loginPage.WrongLoginOrPasswordDiv.Displayed);
            } 
        }

        [Test]
        public void UserClickToRedirectToRegistrationButton_UserRedirectedToRegistrationPage()
        {
            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                var loginPage = new LoginPage(driver);
                loginPage.Navigate();
                loginPage.RedirectToRegistrationDiv.Click();

                _verifyUserIsRedirected("registration", driver);
            }
        }
    }
}

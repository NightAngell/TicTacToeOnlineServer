using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToeSeleniumTests
{
    abstract class SeleniumTestsBase
    {
        public static class SeleniumConfig
        {
            public static DriverType SelectedDriver { get; set; } = DriverType.Chrome;
            public static TimeSpan BaseTimeout { get; set; } = TimeSpan.FromSeconds(20);
            #warning Remember to change pathToDrivers to your own
            public static string pathToDrivers = @"C:\Users\Mateusz Sobo\Desktop\Studium dyplomowe\TicTacToeOnlineServer\TicTacToeSeleniumTests\bin\Debug\netcoreapp2.1";
        }

        readonly IDictionary<RemoteWebDriver, WebDriverWait> _webDriverWaiters 
            = new Dictionary<RemoteWebDriver, WebDriverWait>();

        protected RemoteWebDriver _getNewInstanceOfRequiredDriver()
        {
            if (SeleniumConfig.SelectedDriver == DriverType.Firefox)
                return new FirefoxDriver(SeleniumConfig.pathToDrivers);
            if (SeleniumConfig.SelectedDriver == DriverType.Edge)
                return new EdgeDriver(SeleniumConfig.pathToDrivers);

            return new ChromeDriver(SeleniumConfig.pathToDrivers);
        }

        protected WebDriverWait _getWebDriverWait(RemoteWebDriver driver, TimeSpan timeout)
        {
            if (_webDriverWaiters.ContainsKey(driver))
            {
                return _webDriverWaiters[driver];
            }
            else
            {
                var waiter = new WebDriverWait(driver, timeout);
                _webDriverWaiters.Add(driver, waiter);
                return waiter;
            }
        }

        protected WebDriverWait _getWebDriverWait(RemoteWebDriver driver)
        {
            return _getWebDriverWait(driver, SeleniumConfig.BaseTimeout);
        }

        protected void _verifyIfElementExistAfter(TimeSpan timeout, RemoteWebDriver driver, By by)
        {
            _getWebDriverWait(driver, timeout).Until(ExpectedConditions.ElementExists(by));
        }

        protected void _verifyIfElementExist(RemoteWebDriver driver, By by)
        {
            _verifyIfElementExistAfter(SeleniumConfig.BaseTimeout, driver, by);
        }

        /// <summary>
        /// For example if http://localhost:4200/login is our URL, then partOfUrlAfterBase = "login"
        /// </summary>
        protected void _verifyUserIsRedirected(string partOfUrlAfterBase, RemoteWebDriver driver)
        {
            _getWebDriverWait(driver).Until(ExpectedConditions.UrlContains(partOfUrlAfterBase));
        }

        protected void _waitUntilUserIsRedirected(string partOfUrlAfterBase, RemoteWebDriver driver)
        {
            _getWebDriverWait(driver).Until(ExpectedConditions.UrlContains(partOfUrlAfterBase));
        }

        protected void _waitForElement(RemoteWebDriver driver, By by)
        {
            _getWebDriverWait(driver).Until(ExpectedConditions.ElementExists(by));
        }

        protected void _verifyTextIsInElement(RemoteWebDriver driver, IWebElement element, string text)
        {
            _getWebDriverWait(driver)
                     .Until(
                        ExpectedConditions.TextToBePresentInElement(element, text)
                    );
        }

        protected void _waitForTextInElement(RemoteWebDriver driver, IWebElement element, string text)
        {
            _getWebDriverWait(driver)
                     .Until(
                        ExpectedConditions.TextToBePresentInElement(element, text)
                    );
        }

        public enum DriverType
        {
            Firefox,
            Chrome,
            Edge,
        }
    }
}

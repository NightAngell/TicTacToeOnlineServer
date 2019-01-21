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
        protected RemoteWebDriver _getNewInstanceOfRequiredDriver()
        {
            //return new EdgeDriver(SeleniumConfig.pathToDrivers);
            //return new FirefoxDriver(SeleniumConfig.pathToDrivers);
            return new ChromeDriver(SeleniumConfig.pathToDrivers);
        }

        protected void _verifyIfElementExistAfter(TimeSpan timeout, RemoteWebDriver driver, By by)
        {
            new WebDriverWait(driver, timeout)
                    .Until(ExpectedConditions.ElementExists(by));
        }

        /// <summary>
        /// For example if http://localhost:4200/login is our URL, then partOfUrlAfterBase = "login"
        /// </summary>
        protected void _verifyUserIsRedirected(string partOfUrlAfterBase, RemoteWebDriver driver)
        {
            new WebDriverWait(driver, SeleniumConfig.BaseTimeout)
                    .Until(ExpectedConditions.UrlContains(partOfUrlAfterBase));
        }

        protected void _waitForElement(RemoteWebDriver driver, By by)
        {
            new WebDriverWait(driver, SeleniumConfig.BaseTimeout)
                    .Until(ExpectedConditions.ElementExists(by));
        }

        public static class SeleniumConfig
        {
            public static TimeSpan BaseTimeout { get; set; } = TimeSpan.FromSeconds(40);
            #warning Remember to change pathToDrivers to your own
            public static string pathToDrivers = @"C:\Users\Mateusz Sobo\Desktop\Studium dyplomowe\TicTacToeOnlineServer\TicTacToeSeleniumTests\bin\Debug\netcoreapp2.1";
        }
    }
}

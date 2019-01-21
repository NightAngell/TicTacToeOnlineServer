using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumExtras.PageObjects;

namespace TicTacToeSeleniumTests.PageObjects
{
    abstract class PageObjectBase
    {
        protected readonly RemoteWebDriver _driver;
        protected string _baseAddress = "http://localhost:4200/";
        protected string _addressAfterBaseAddress = "";

        public PageObjectBase(RemoteWebDriver driver, string addressAfterBaseAddress)
        {
            _driver = driver;
            _addressAfterBaseAddress = addressAfterBaseAddress;
            PageFactory.InitElements(driver, this);
        }

        public void Navigate()
        {
            _driver
                .Navigate()
                .GoToUrl($"{_baseAddress}{_addressAfterBaseAddress}");
        }
    }
}
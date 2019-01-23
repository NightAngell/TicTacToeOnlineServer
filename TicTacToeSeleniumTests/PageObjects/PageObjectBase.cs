using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumExtras.PageObjects;

namespace TicTacToeSeleniumTests.PageObjects
{
    abstract class PageObjectBase
    {
        public RemoteWebDriver Driver { get; protected set; }
        //TODO add check if base address ane with "/" char
        public string BaseAddress { get; protected set; } = "http://localhost:4200/";
        public string AddressAfterBaseAddress { get; protected set; } = "";

        public PageObjectBase(RemoteWebDriver driver, string addressAfterBaseAddress)
        {
            Driver = driver;
            AddressAfterBaseAddress = addressAfterBaseAddress;
            PageFactory.InitElements(driver, this);
        }

        public void Navigate()
        {
            Driver
                .Navigate()
                .GoToUrl($"{BaseAddress}{AddressAfterBaseAddress}");
        }
    }
}
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeSeleniumTests.PageObjects;
using TicTacToeServer.Database;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using TicTacToeServer.Models;

namespace TicTacToeSeleniumTests.Auth
{
    [TestFixture]
    class RegistrationTests : SeleniumTestsBase
    {
        [Test]
        public void WeHaveValidLoginAndPassword_UserRegistrated()
        {
            const string validEmail = "valid@email.com";
            const string validPassword = "SuperSecureP@ssw0rd";

            using (var driver = _getNewInstanceOfRequiredDriver())
            {
                var registrationPage = new RegistrationPage(driver);
                registrationPage.Navigate();
                registrationPage.LoginInput.SendKeys(validEmail);
                registrationPage.PasswordInput.SendKeys(validPassword);
                registrationPage.SubmitButton.Submit();

                _verifyUserIsRedirected("login", driver);
            }

            DbContextOptionsBuilder<Db> optionsBuilder = new DbContextOptionsBuilder<Db>();
            optionsBuilder.UseSqlServer(Db.connectionString);

            using (var db = new Db(optionsBuilder.Options))
            {
                var userToDelete = db.Users.FirstOrDefault(x => x.Email == validEmail);
                Assert.NotNull(userToDelete);
                if(userToDelete != null)
                {
                    db.Remove(userToDelete);
                    db.SaveChanges();
                }
            }
        }
    }
}

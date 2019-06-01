using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.Services;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestServiceTask
{
    class UserServiceTest
    {
        private IOptions<AppSettings> config;

        [SetUp]
        public void Setup()
        {
            config = Options.Create(new AppSettings
            {
                Secret = "BLOOD FOR THE BLOOD GOD"
            });
        }

        [Test]
        public void Registration()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Registration))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var usersService = new UserService(context, config);
                var gimli = new RegisterPostModel
                {
                    Email = "gimli@moria.com",
                    FirstName = "Gimli",
                    LastName = "Bronzebeard",
                    Password = "1234567",
                    Username = "gimli",
                };
                var result = usersService.Register(gimli);

                Assert.IsNotNull(result);
                Assert.AreEqual(gimli.Username, result.Username);

            }
        }

        [Test]
        public void Authentification()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Authentification))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var usersService = new UserService(context, config);

                var added = new RegisterPostModel
                {
                    Email = "gandalf@yahoo.com",
                    FirstName = "Gandalf",
                    LastName = "The White",
                    Password = "12345678",
                    Username = "gman"
                };
                usersService.Register(added);

                var loggedIn = new LoginPostModel
                {
                    Username = "gman",
                    Password = "12345678"

                };
                var result = usersService.Authenticate(added.Username, added.Password);

                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Id);
                Assert.AreEqual(loggedIn.Username, result.Username);
            }
        }

        [Test]
        public void GetAll()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAll))//nameof(ValidUsernameAndPasswordShouldLoginSuccessfully))// "ValidUsernameAndPasswordShouldLoginSuccessfully")
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var usersService = new UserService(context, config);

                var added = new RegisterPostModel
                {
                    Email = "aragorn@yahoo.com",
                    FirstName = "Aragorn",
                    LastName = "True King",
                    Password = "12345678",
                    Username = "kingman"
                };
                usersService.Register(added);

                var result = usersService.GetAll();

                Assert.AreEqual(1, result.Count());

            }
        }

    }
}

using lab2_restapi_1205_taskmgmt.Constants;
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

        [Test]
        public void Upsert()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()                
              .UseInMemoryDatabase(databaseName: nameof(Upsert))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var usersService = new UserService(context, config);
                var roleService = new RoleService(context, config);

                var regularPost = new RolePostModel
                {
                    Title = RoleConstants.REGULAR
                };
                var regular = roleService.Create(regularPost);
                context.Entry(regular).State = EntityState.Detached;
                var adminPost = new RolePostModel
                {
                    Title = RoleConstants.ADMIN
                };
                var admin = roleService.Create(adminPost);
                context.Entry(admin).State = EntityState.Detached;

                Role role = new Role
                {
                    Title = RoleConstants.ADMIN
                };
                HistoryUserRole history = new HistoryUserRole
                {
                    Role = role,
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
                {
                    history
                };
                User aragorn = new User
                {
                    Username = "kingman",
                    History = list,
                    CreatedAt = DateTime.Now
                };


                Role roleLegolas = new Role
                {
                    Title = RoleConstants.REGULAR
                };
                HistoryUserRole historyLegolas = new HistoryUserRole
                {
                    Role = roleLegolas,
                };
                List<HistoryUserRole> listLegolas = new List<HistoryUserRole>
                {
                    historyLegolas
                };
                User legolas = new User
                {
                    Username = "legolas",
                    History = listLegolas,
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(legolas);
                context.SaveChanges();
            }

            using (var context = new TasksDbContext(options)) {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var roleService = new RoleService(context, config);
                var usersService = new UserService(context, config);

                var adminPost = new RolePostModel
                {
                    Title = RoleConstants.ADMIN
                };
                var admin = roleService.Create(adminPost);
                context.Entry(admin).State = EntityState.Detached;

                Role role = new Role
                {
                    Title = RoleConstants.ADMIN
                };
                HistoryUserRole history = new HistoryUserRole
                {
                    Role = role,
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
                {
                    history
                };
                User aragorn = new User
                {
                    Username = "kingman",
                    History = list,
                    CreatedAt = DateTime.Now

                };

                var toBeAdded = new RegisterPostModel
                {
                    Email = "legolas@yahoo.com",
                    FirstName = "Legolas",
                    LastName = "Bowmaster",
                    Password = "12345678",
                    Username = "legolas"
                };
                //usersService.Register(toBeAdded);

                var toUpdateWith = new UserPostModel
                {
                    FirstName = "Gimli",
                    LastName = "Axeman",
                    UserName = "gimli",
                    Email = "gimli@gmail.com",
                    UserRole = RoleConstants.ADMIN
                };                                              
                
                //User legolas = context.Users
                //    .AsNoTracking()
                //    .Include(x => x.History)                    
                //    .ThenInclude(x => x.Role)
                //    .AsNoTracking()
                //    .FirstOrDefault(x => x.FirstName.Equals(toBeAdded.FirstName));

                //context.Entry(legolas).State = EntityState.Detached;
                //context.SaveChanges();

                var lala = context.ChangeTracker.Entries()
                    .Where(t => t.State == EntityState.Unchanged);

                //context.Entry(test).State = EntityState.Detached;
                usersService.Upsert(1, toUpdateWith, aragorn);

                //User gimli = context.Users
                //    .AsNoTracking()
                //    .Include(x => x.History)
                //    .ThenInclude(x => x.Role)
                //    .FirstOrDefault(x => x.Id.Equals(legolas.Id));

                Assert.AreNotEqual(gimli.FirstName, legolas.FirstName);
                Assert.AreNotEqual(gimli.Username, legolas.Username);
                Assert.AreNotEqual(gimli.LastName, legolas.LastName);
                Assert.AreNotEqual(gimli.Email, legolas.Email);
                Assert.AreNotEqual(RoleConstants.REGULAR, gimli.History.FirstOrDefault().Role.Title);
            }
        }

        [Test]
        public void GetById()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetById))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var usersService = new UserService(context, config);
                var second = new RegisterPostModel
                {
                    Email = "legolas@yahoo.com",
                    FirstName = "Legolas",
                    LastName = "Bowmaster",
                    Password = "12345678",
                    Username = "legolas"
                };
                usersService.Register(second);
                User expected = context.Users.AsNoTracking()
                    .Include(x => x.History)
                    .ThenInclude(x => x.Role)
                    .FirstOrDefault(x => x.FirstName.Equals(second.FirstName));
                UserGetModel actual = usersService.GetById(expected.Id);

                Assert.AreEqual(expected.Username, actual.Username);
                Assert.AreEqual(expected.Id, actual.Id);
            }
        }

        [Test]
        public void Delete()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetById))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var usersService = new UserService(context, config);
                Role role = new Role
                {
                    Id = 3,
                    Title = RoleConstants.ADMIN
                };

                HistoryUserRole history = new HistoryUserRole
                {
                    AllocatedAt = DateTime.Now,
                    RoleId = 3,
                    Role = role,
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
                {
                    history
                };

                User aragorn = new User
                {
                    Username = "kingman",
                    History = list,
                    CreatedAt = DateTime.Now

                };
                //aragorn.History.Add(history);

                var legolas = new RegisterPostModel
                {
                    Email = "legolas@yahoo.com",
                    FirstName = "Legolas",
                    LastName = "Bowmaster",
                    Password = "12345678",
                    Username = "legolas"
                };
                usersService.Register(legolas);
                User expected = context.Users.AsNoTracking()
                    .Include(x => x.History)
                    .ThenInclude(x => x.Role)
                    .FirstOrDefault(x => x.FirstName.Equals(legolas.FirstName));

                usersService.Delete(expected.Id, aragorn);

                Assert.IsNull(usersService.GetById(expected.Id));
            }
        }

    }
}

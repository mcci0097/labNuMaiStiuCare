using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.Services;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestServiceTask
{
    class RoleServiceTests
    {
        private IOptions<AppSettings> config;

        [SetUp]
        public void Setup()
        {
            config = Options.Create(new AppSettings
            {
                Secret = "My armor is contempt, my shield is disgust, my sword is hatred, in the Emperor's name let none survive"
            });
        }

        [Test]
        public void Create()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Create))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var roleService = new RoleService(context, config);

                var toAdd = new RolePostModel
                {
                    Title = "GOD"
                };

                Role role = roleService.Create(toAdd);

                Assert.AreEqual(toAdd.Title, role.Title);
                Assert.IsNotNull(roleService.GetById(role.Id));
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
                var roleService = new RoleService(context, config);
                var toAdd = new RolePostModel
                {
                    Title = "GOD"
                };

                var toUpdate = new RolePostModel
                {
                    Title = "Sclave Master"
                };

                Role role = roleService.Create(toAdd);
                context.Entry(role).State = EntityState.Detached;
                Role updated = roleService.Upsert(role.Id, toUpdate);

                Assert.AreNotEqual(role.Title, updated.Title);
            }
        }

        [Test]
        public void Delete()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Delete))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var roleService = new RoleService(context, config);
                var toAdd = new RolePostModel
                {
                    Title = "GOD"
                };
                Role role = roleService.Create(toAdd);
                roleService.Delete(role.Id);

                Assert.IsNull(context.Roles.Find(role.Id));
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
                var roleService = new RoleService(context, config);
                var toAdd = new RolePostModel
                {
                    Title = "GOD"
                };
                Role expected = roleService.Create(toAdd);
                Role actual = roleService.GetById(expected.Id);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void GetAll()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAll))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var roleService = new RoleService(context, config);
                var first = new RolePostModel
                {
                    Title = "God"
                };
                var second = new RolePostModel
                {
                    Title = "Slave Master"
                };
                var third = new RolePostModel
                {
                    Title = "Matroana"
                };
                var fourth = new RolePostModel
                {
                    Title = "Shobolan"
                };
                var fifth = new RolePostModel
                {
                    Title = "Tod Howard"
                };
                roleService.Create(first);
                roleService.Create(second);
                roleService.Create(third);
                roleService.Create(fourth);
                roleService.Create(fifth);
                context.SaveChanges();

                PaginatedList<RoleGetModel> populated = roleService.GetAll(1);
                PaginatedList<RoleGetModel> empty = roleService.GetAll(2);

                Assert.AreEqual(5, populated.Entries.Count);
                Assert.Zero(empty.Entries.Count);

            }
        }
    }
}

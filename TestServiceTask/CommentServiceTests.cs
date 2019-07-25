using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestServiceTask
{
    class CommentServiceTests
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
        public void ValidGetAllShouldDisplayAllComments()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(ValidGetAllShouldDisplayAllComments))
              .Options;

            using (var context = new TasksDbContext(options))
            {
                var commentService = new CommentService(context);

                var added = new lab2_restapi_1205_taskmgmt.ViewModels.CommentPostModel

                {
                    Text = "Write a Book",
                    Important = true,

                };
                var added2 = new lab2_restapi_1205_taskmgmt.ViewModels.CommentPostModel

                {
                    Text = "Read a Book",
                    Important = false,

                };

                commentService.Create(added);
                commentService.Create(added2);
               //test
               //test
               dsada
               dsadadsa
               dsadsa

                var result = commentService.GetAll(string.Empty);
                Assert.AreEqual(0, result.Count());

            }
        }
    }
}

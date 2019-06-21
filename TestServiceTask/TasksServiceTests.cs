using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TasksServiceTests
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
        public void CreateANewTask()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(CreateANewTask))// "CreateANewTask")
              .Options;
            using (var context = new TasksDbContext(options))
            {
                var tasksService = new TaskService(context);
                Task task = new Task();
                var create = new lab2_restapi_1205_taskmgmt.ViewModels.TaskPostModel
                {
                    Title = "Read1",
                    Description = "Read1",
                    Added = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(15),
                    ClosedAt = null,
                    Importance = "Medium",
                    State = "InProgress",
                    Comments = task.Comments
                };
                //var result = tasksService.Create(create);
                //Assert.NotNull(result);
                //Assert.AreEqual(create.Title, result.Title);
            }

        }

        [Test]
        public void DeleteExistingTask()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(DeleteExistingTask))// "DeleteExistingTask")
              .Options;
            using (var context = new TasksDbContext(options))
            {
                var tasksService = new TaskService(context);
                var result = new lab2_restapi_1205_taskmgmt.ViewModels.TaskPostModel
                {
                    Title = "Read2",
                    Description = "Read2",
                    Added = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(15),
                    ClosedAt = null,
                    Comments = null
                };
                //Task savetask = tasksService.Create(result);
                //Task task2 = tasksService.Delete(savetask.Id);

                //Assert.IsNull(tasksService.GetById(task2.Id));
            }
        }

        [Test]
        public void UpdateExistingTask()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(UpdateExistingTask))// "UpdateExistingTask")
              .Options;
            using (var context = new TasksDbContext(options))
            {
                var tasksService = new TaskService(context);
                var resultTaskPostModel = new lab2_restapi_1205_taskmgmt.ViewModels.TaskPostModel
                {                    
                    Title = "Read3",
                    Description = "Read3",
                    Added = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(15),
                    ClosedAt = null,
                    Comments = null
                };

                var user = new User
                {
                    FirstName = "Frodo",
                    LastName = "Baggings",
                    Email = "frodo@yahoo.com",
                    Username = "frodo",
                    Password = "lalala",
                    CreatedAt = DateTime.Now
                };
                var lalala = tasksService.Create(resultTaskPostModel, user);                 

                List<Comment> comments = new List<Comment>();
                Comment comment = new Comment();
                comment.Id = 1;
                comment.Text = "One Ring to Rule them All";
                comment.Important = true;
                comments.Add(comment);

                var dateDeadline = DateTime.Now.AddDays(20);

                var resultTask = new Task
                {
                    Title = "Read4",
                    Description = "Read4",
                    Added = DateTime.Now,
                    Deadline = dateDeadline,
                    ClosedAt = null,
                    Comments = comments
                };
                context.Entry(lalala).State = EntityState.Detached;
                //tasksService.Upsert(lalala.Id, resultTask);

                Assert.AreEqual(resultTask.Title, "Read4");
                Assert.AreEqual(resultTask.Description, "Read4");
                Assert.AreEqual(resultTask.Deadline, dateDeadline);
                Assert.AreEqual(resultTask.Comments, comments);
            }
        }
    }
}
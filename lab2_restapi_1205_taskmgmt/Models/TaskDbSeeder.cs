using System;
using System.Linq;

namespace lab2_restapi_1205_taskmgmt.Models
{
    public static class TaskDbSeeder
    {
        public static void Initialize(TasksDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any tasks.
            if (context.Tasks.Any())
            {
                return;   // DB has been seeded
            }

            context.Tasks.AddRange(
                new Task
                {
                    Description = "Procure honorable women",
                    Title = "Rise to power via acquisition",
                    Added = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(20),
                    Importance = 0,
                    State = 0
                },

                new Task
                {
                    Description = "Acquire wealth",
                    Title = "Rise to power via procurement",
                    Added = DateTime.Now,
                    Deadline = DateTime.Now.AddMonths(3),
                    Importance = 0,
                    State = 0
                },

                new Task
                {
                    Description = "Establish chemical romance for the customers",
                    Title = "Rise to power via chemical addiction",
                    Added = DateTime.Now,
                    Deadline = DateTime.Now.AddMonths(3),
                    Importance = 0,
                    State = 0
                }

            );
            context.SaveChanges(); // commit transaction
        }
    }
}
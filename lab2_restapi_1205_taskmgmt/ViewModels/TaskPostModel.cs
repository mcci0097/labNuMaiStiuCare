using lab2_restapi_1205_taskmgmt.Models;
using System;
using System.Collections.Generic;
using Task = lab2_restapi_1205_taskmgmt.Models.Task;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class TaskPostModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? ClosedAt { get; set; }

        public string Importance { get; set; }

        public string State { get; set; }

        public List<Comment> Comments { get; set; }

        public static Task ToTask(TaskPostModel task)

        {
            Importance importance = Models.Importance.Low;
            if (task.Importance == "Medium")
            {
                importance = Models.Importance.Medium;
            }
            else if (task.Importance == "High")
            {
                importance = Models.Importance.High;
            }
            State state = Models.State.Open;
            if (task.State == "InProgress")
            {
                state = Models.State.InProgress;
            }
            else if (task.State == "Closed")
            {
                state = Models.State.Closed;
            }

            return new Task
            {
                Title = task.Title,
                Description = task.Description,
                Added = task.Added,
                Deadline = task.Deadline,
                ClosedAt = task.ClosedAt,
                Importance = importance,
                State = state,
                Comments = task.Comments
            };
        }
    }
}
using lab2_restapi_1205_taskmgmt.Models;
using System;
using System.Collections.Generic;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class TaskGetModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Deadline { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string Importance { get; set; }
        public string State { get; set; }
        public List<Comment> Comments { get; set; }

        public static TaskGetModel FromTask(Models.Task task)
        {
            return new TaskGetModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Added = task.Added,
                Deadline = task.Deadline,
                NumberOfComments = task.Comments.Count,
                ClosedAt = task.ClosedAt,
                Importance = task.Importance.ToString(),
                State = task.State.ToString(),
                Comments = task.Comments
            };
        }
    }
}
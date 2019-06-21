using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Task = lab2_restapi_1205_taskmgmt.Models.Task;

namespace lab2_restapi_1205_taskmgmt.Services
{
    public interface ICommentService
    {
        IEnumerable<CommentGetModel> GetAll(String filter);
    }

    public class CommentService : ICommentService
    {
        private TasksDbContext context;

        public CommentService(TasksDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<CommentGetModel> GetAll(String filter)
        {
            IQueryable<Task> result = context.Tasks.Include(c => c.Comments);

            List<CommentGetModel> resultComments = new List<CommentGetModel>();
            List<CommentGetModel> resultCommentsAll = new List<CommentGetModel>();

            foreach (Task task in result)
            {
                task.Comments.ForEach(c =>
                {
                    if (c.Text == null || filter == null)
                    {
                        CommentGetModel comment = new CommentGetModel
                        {
                            Id = c.Id,
                            Important = c.Important,
                            Text = c.Text,
                            TaskId = task.Id
                        };
                        resultCommentsAll.Add(comment);
                    }
                    else if (c.Text.Contains(filter))
                    {
                        CommentGetModel comment = new CommentGetModel
                        {
                            Id = c.Id,
                            Important = c.Important,
                            Text = c.Text,
                            TaskId = task.Id
                        };
                        resultComments.Add(comment);
                    }
                });
            }
            if (filter == null)
            {
                return resultCommentsAll;
            }
            return resultComments;
        }
        public Comment Create(CommentPostModel commentPostModel)
        {
            Comment toAdd = CommentPostModel.ToComment(commentPostModel);

            context.Comments.Add(toAdd);
            context.SaveChanges();
            return toAdd;

        }
    }
}
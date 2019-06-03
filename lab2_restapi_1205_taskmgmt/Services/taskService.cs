using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Task = lab2_restapi_1205_taskmgmt.Models.Task;

namespace lab2_restapi_1205_taskmgmt.Services
{
    public interface ITaskService
    {
        PaginatedList<TaskGetModel> GetAll(int page, DateTime? from = null, DateTime? to = null);

        Task Create(TaskPostModel task, User addedBy);

        Task Upsert(int id, Task task);

        Task Delete(int id);

        Task GetById(int id);
    }

    public class TaskService : ITaskService
    {
        private TasksDbContext context;

        public TaskService(TasksDbContext context)
        {
            this.context = context;
        }

        public Task Create(TaskPostModel task, User addedBy)
        {
            Task toAdd = TaskPostModel.ToTask(task);

            toAdd.Owner = addedBy;

            context.Tasks.Add(toAdd);
            context.SaveChanges();
            return toAdd;
        }

        public Task Delete(int id)
        {
            var existing = context.Tasks.Include(f => f.Comments).FirstOrDefault(task => task.Id == id);
            if (existing == null)
            {
                return null;
            }
            context.Tasks.Remove(existing);
            context.SaveChanges();

            return existing;
        }

        public PaginatedList<TaskGetModel> GetAll(int page, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Task> result = context
                .Tasks
                .OrderBy(t => t.Id)
                .Include(f => f.Comments);
            PaginatedList<TaskGetModel> paginatedList = new PaginatedList<TaskGetModel>();
            paginatedList.CurrentPage = page;

            //if there are more includes use thenInclude

            if (from != null)
            {
                result = result.Where(f => f.Deadline >= from);
            }

            if (to != null)
            {
                result = result.Where(f => f.Deadline <= to);
            }
            
            paginatedList.NumberOfPages = (result.Count() - 1) / PaginatedList<TaskGetModel>.EntriesPerPage + 1;
            result = result
                .Skip((page - 1) * PaginatedList<TaskGetModel>.EntriesPerPage)
                .Take(PaginatedList<TaskGetModel>.EntriesPerPage);
            paginatedList.Entries = result.Select(t => TaskGetModel.FromTask(t)).ToList();

            return paginatedList;
        }

        public Task GetById(int id)
        {
            return context.Tasks
                 .Include(f => f.Comments)
                 .FirstOrDefault(f => f.Id == id);
        }

        public Task Upsert(int id, Task task)
        {
            var existing = context.Tasks.AsNoTracking().FirstOrDefault(f => f.Id == id);
            if (existing == null)
            {
                context.Tasks.Add(task);
                context.SaveChanges();
                return task;
            }
            if (task.State.ToString() == "Closed")
            {
                task.ClosedAt = DateTime.Now;
            }
            else
            {
                task.ClosedAt = null;
            }
            task.Id = id;
            context.Tasks.Update(task);
            context.SaveChanges();
            return task;
        }
    }
}
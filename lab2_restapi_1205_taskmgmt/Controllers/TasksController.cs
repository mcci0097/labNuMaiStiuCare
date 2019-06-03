using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.Services;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Task = lab2_restapi_1205_taskmgmt.Models.Task;

namespace lab2_restapi_1205_taskmgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        //  private TasksDbContext context;
        private ITaskService taskService;
        private IUserService userService;

        public TasksController(ITaskService taskService, IUserService userService)
        {
            this.taskService = taskService;
            this.userService = userService;
        }

        // GET: api/Task

        /// <summary>
        ///
        /// Returns all task within given timeframe
        /// </summary>
        /// <param name="from">Optional, starting point</param>
        /// <param name="to">Optional, ending points</param>
        /// <returns>List of tasks</returns>
        ///<remarks>
        ///{
        ///"id": 6,
        ///"title": "asdasdfsaddf",
        ///"description": "sdf",
        ///"added": "2011-01-01T00:00:00",
        ///"deadline": "2012-01-01T00:00:00",
        ///"numberOfComments": 0,
        ///"closedAt": null,
        ///"importance": "High",
        ///"state": "Open",
        ///"comments": []
        /// }
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        //   public IEnumerable<Models.Task> Get([FromQuery]DateTime? from, [FromQuery]DateTime? to)
        public PaginatedList<TaskGetModel> Get([FromQuery]DateTime? from, [FromQuery]DateTime? to, [FromQuery] int page = 1)
        {

            page = Math.Max(page, 1);
            return taskService.GetAll(page, from, to);

            //IQueryable<Task> result = context.Tasks.Include(f => f.Comments);
            ////if there are more includes use thenInclude

            //if (from == null && to == null)
            //{
            //    return result;
            //}
            //if (from != null)
            //{
            //    result = result.Where(f => f.Deadline >= from);
            //}

            //if (to != null)
            //{
            //    result = result.Where(f => f.Deadline <= to);
            //}
            //return result;
        }

        // GET: api/Task/5
        /// <summary>
        /// Get given task when task ID is Specified
        /// </summary>
        /// <param name="id">Need parameter</param>
        /// <returns>The task inquired</returns>
        ///<remarks>
        ///{
        ///"id": 6,
        ///"title": "asdasdfsaddf",
        ///"description": "sdf",
        ///"added": "2011-01-01T00:00:00",
        ///"deadline": "2012-01-01T00:00:00",
        ///"numberOfComments": 0,
        ///"closedAt": null,
        ///"importance": "High",
        ///"state": "Open",
        ///"comments": []
        /// }
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Regular, Admin")]
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var found = taskService.GetById(id);
            if (found == null)
            {
                return NotFound();
            }

            return Ok(found);
        }

        // POST: api/Tasks
        /// <summary>
        /// Returns all tasks
        /// </summary>
        /// <param name="task"></param>
        ///

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Regular")]
        [HttpPost]
        public void Post([FromBody] TaskPostModel task)
        {
            User addedBy = userService.GetCurentUser(HttpContext);
            taskService.Create(task, addedBy);            
            //if (!ModelState.IsValid)
            //{
            //}
            //context.Tasks.Add(task);
            //context.SaveChanges();
        }

        // PUT: api/Tasks/5
        /// <summary>
        /// Edit given task when specifying id. Adds if not exists
        /// </summary>
        /// <param name="id">The id of the task</param>
        /// <param name="task">The type of returned field</param>
        /// <returns>The task</returns>
        /// ///<remarks>
        ///{
        ///"id": 6,
        ///"title": "asdasdfsaddf",
        ///"description": "sdf",
        ///"added": "2011-01-01T00:00:00",
        ///"deadline": "2012-01-01T00:00:00",
        ///"numberOfComments": 0,
        ///"closedAt": null,
        ///"importance": "High",
        ///"state": "Open",
        ///"comments": []
        /// }
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Regular")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Task task)
        {
            var result = taskService.Upsert(id, task);
            return Ok(result);
            //var existing = context.Tasks.AsNoTracking().FirstOrDefault(f => f.Id == id);
            //if (existing == null)
            //{
            //    context.Tasks.Add(task);
            //    context.SaveChanges();
            //    return Ok(task);
            //}
            //task.Id = id;
            //context.Tasks.Update(task);
            //context.SaveChanges();
            //return Ok(task);
        }

        /// <summary>
        /// Deletes task
        /// </summary>
        /// <param name="id">Id of task </param>
        /// <returns>Deleted task</returns>
        ///<remarks>
        ///{
        ///"id": 6,
        ///"title": "asdasdfsaddf",
        ///"description": "sdf",
        ///"added": "2011-01-01T00:00:00",
        ///"deadline": "2012-01-01T00:00:00",
        ///"numberOfComments": 0,
        ///"closedAt": null,
        ///"importance": "High",
        ///"state": "Open",
        ///"comments": []
        /// }
        /// </remarks>

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Regular")]
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = taskService.Delete(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
            //var existing = context.Tasks.FirstOrDefault(task => task.Id == id);
            //if (existing == null)
            //{
            //    return NotFound();
            //}
            //context.Tasks.Remove(existing);
            //context.SaveChanges();
            //return Ok();
        }
    }
}
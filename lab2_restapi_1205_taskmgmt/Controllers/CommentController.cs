using lab2_restapi_1205_taskmgmt.Services;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace lab2_restapi_1205_taskmgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        ///<remarks>
        ///{
        ///"id": 1,
        ///"text": "searchword  - something else",
        ///"important": true,
        ///"TaskId": 7
        ///}
        ///</remarks>
        /// <summary>
        /// Return the searched string
        /// </summary>
        /// <param name="filter">Optional, filtered by text</param>
        /// <returns>List of comments</returns>

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        // GET: api/Comments
        [HttpGet]
        public IEnumerable<CommentGetModel> GetAll([FromQuery]String filter)
        {
            return commentService.GetAll(filter);
        }
    }
}
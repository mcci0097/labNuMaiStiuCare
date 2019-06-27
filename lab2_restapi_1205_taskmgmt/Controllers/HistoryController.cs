using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using lab2_restapi_1205_taskmgmt.Services;

namespace lab2_restapi_1205_taskmgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private IUserService userService;

        public HistoryController(IUserService userservice)
        {
            userService = userservice;
        }

        /// <summary>
        /// Gets a role history for the user
        /// GET: api/History/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // 
        [Authorize(Roles = "Admin, User_Manager")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var found = userService.GetHistoryById(id);
            if (found == null)
            {
                return NotFound();
            }
            return Ok(found);
        }

    }
}
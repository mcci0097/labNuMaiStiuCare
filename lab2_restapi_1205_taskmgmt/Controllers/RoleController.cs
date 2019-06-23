using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lab2_restapi_1205_taskmgmt.Services;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lab2_restapi_1205_taskmgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private IRoleService roleService;

        public RoleController(IRoleService roleservice)
        {
            roleService = roleservice;
        }

        // GET: api/Role
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public PaginatedList<RoleGetModel> Get([FromQuery] int page = 1)
        {
            page = Math.Max(page, 1);
            return roleService.GetAll(page);
        }

        // GET: api/Role/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetRole")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetID(int id)
        {
            var found = roleService.GetById(id);
            if (found == null)
            {
                return NotFound();
            }

            return Ok(found);
        }

        // POST: api/Role
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] RolePostModel role)
        {            
            roleService.Create(role);
            return Ok();
        }

        // PUT: api/Role/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromBody] RolePostModel role)
        {
            var result = roleService.Upsert(id, role);
            return Ok(result);
        }

        // DELETE: api/ApiWithActions/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var result = roleService.Delete(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}

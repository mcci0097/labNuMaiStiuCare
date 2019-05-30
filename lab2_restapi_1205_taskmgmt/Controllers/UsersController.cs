using System;
using System.Collections.Generic;
using System.Linq;
using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.Services;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lab2_restapi_1205_taskmgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userservice)
        {
            userService = userservice;
        }
        /// <summary>
        /// Login for user 
        /// </summary>
        /// <remarks>
        ///            {
        ///            "username":"pop91",
        ///            "password":"pop123456"
        ///            }
        /// 
        /// 
        /// </remarks>
        /// <param name="loginModel">Enter username and password</param>
        /// <returns>Return username , email and token</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginPostModel loginModel)
        {
            var user = userService.Authenticate(loginModel.Username, loginModel.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
        /// <summary>
        /// Register a user in the database
        /// </summary>
        /// <remarks>
        ///     {
        ///         "firstName":"Pop",
        ///         "lastName":"Mihai",
        ///         "username":"pop91",
        ///         "email":"pop@yahoo.com",
        ///         "password":"pop123456"
        ///        }
        /// </remarks>
        /// <param name="registerModel">Introduce firstname, lastname,username,email and password</param>
        /// <returns>Inserted user in database</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterPostModel registerModel)
        {
            var user = userService.Register(registerModel);
            if (user == null)
            {
                return BadRequest(new { ErrorMessage ="Username already exists"});
            }
            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = userService.GetAll();
            return Ok(users);
        }
    }
}

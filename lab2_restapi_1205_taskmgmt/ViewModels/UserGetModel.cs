using System;
using System.Collections.Generic;
using System.Linq;
namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class UserGetModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

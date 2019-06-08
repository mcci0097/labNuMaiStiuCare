using lab2_restapi_1205_taskmgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class RolePostModel
    {        
        public string Title { get; set; }

        public static Role ToRole(RolePostModel role)
        {
            return new Role
            {                
                Title = role.Title
            };
        }
    }
}

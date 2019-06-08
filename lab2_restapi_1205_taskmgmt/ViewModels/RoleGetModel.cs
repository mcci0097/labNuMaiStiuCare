using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class RoleGetModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public static RoleGetModel FromRole(Models.Role role)
        {
            return new RoleGetModel
            {
                Id = role.Id,
                Title = role.Title
            };
        }
    }
}

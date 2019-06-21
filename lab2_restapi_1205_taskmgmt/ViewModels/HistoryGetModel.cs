using lab2_restapi_1205_taskmgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class HistoryGetModel
    {
        public string Username { get; set; }        
        public string RoleTitle { get; set; }
        public DateTime? AllocatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }

        public static HistoryGetModel ToHistoryGetModel(HistoryUserRole history)
        {
            return new HistoryGetModel
            {
                Username = history.User.Username,
                RoleTitle = history.Role.Title,
                AllocatedAt = history.AllocatedAt,
                RemovedAt = history.RemovedAt
            };
        }

    }
}

using lab2_restapi_1205_taskmgmt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.Models
{
    public class HistoryUserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User{ get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public DateTime? AllocatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
    }
} 
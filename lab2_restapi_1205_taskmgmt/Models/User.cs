using lab2_restapi_1205_taskmgmt.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.Models
{
    //public enum UserRole {
    //    Regular,
    //    User_Manager,
    //    Admin
    //}

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //[EnumDataType(typeof(UserRole))]
        //public UserRole Role { get; set; }
        public IEnumerable<HistoryUserRole> Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

﻿using lab2_restapi_1205_taskmgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class UserPostModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }

        public static User ToUser(UserPostModel userModel)
        {
            UserRole rol = Models.UserRole.Regular;

            if (userModel.UserRole == "User_Manager")
            {
                rol = Models.UserRole.User_Manager;
            }
            else if (userModel.UserRole == "Admin")
            {
                rol = Models.UserRole.Admin;
            }

            return new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Username = userModel.UserName,
                Email = userModel.Email,
                Role = rol                              
            };
        }
    }

}


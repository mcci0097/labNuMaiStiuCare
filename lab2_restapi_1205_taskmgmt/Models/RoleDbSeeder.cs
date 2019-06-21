using lab2_restapi_1205_taskmgmt.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.Models
{
    public class RoleDbSeeder
    {
        public static void Initialize(TasksDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any tasks.
            if (context.Roles.Any())
            {
                return;   // DB has been seeded
            }

            context.Roles.AddRange(
                new Role
                {
                    Title = RoleConstants.REGULAR,
                },

                                new Role
                                {
                                    Title = RoleConstants.USER_MANAGER,
                                },

                                new Role
                                {
                                    Title = RoleConstants.ADMIN,
                                }

            );

            //context.Users.AddRange(
            //    new User
            //    {
            //        FirstName = "Balanelo",
            //        LastName = "Lukian",
            //        Username = "bman",
            //        Email = "bman@gmail.com",
            //        Password = "1234567",
            //        Role = UserRole.User_Manager
            //    },

            //    new User
            //    {
            //        FirstName = "Radu Allmighty",
            //        LastName = "Allpowerfull",
            //        Username = "king",
            //        Email = "king@gmail.com",
            //        Password = "1234567",
            //        Role = UserRole.Admin
            //    }
            //    );
            context.SaveChanges(); // commit transaction
        }
    }
}

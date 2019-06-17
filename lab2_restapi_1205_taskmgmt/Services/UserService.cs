using lab2_restapi_1205_taskmgmt.Constants;
using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace lab2_restapi_1205_taskmgmt.Services
{

    public interface IUserService
    {
        UserGetModel Authenticate(string username, string password);
        UserGetModel Register(RegisterPostModel register);
        IEnumerable<UserGetModel> GetAll();
        User GetCurentUser(HttpContext httpContext);

        User GetById(int id);
        User Create(UserPostModel userModel);
        User Upsert(int id, UserPostModel userPostModel, User addedBy);
        User Delete(int id, User addedBy);

    }

    public class UserService : IUserService
    {
        private TasksDbContext dbcontext;

        private readonly AppSettings appSettings;

        public UserService(TasksDbContext context, IOptions<AppSettings> appSettings)
        {
            this.dbcontext = context;
            this.appSettings = appSettings.Value;
        }

        public UserGetModel Authenticate(string username, string password)
        {
            var user = dbcontext.Users
                .SingleOrDefault(x => x.Username == username && x.Password == ComputeSha256Hash(password));

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = tokenHandler.WriteToken(token)

            };


            return result;
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            // TODO: also use salt
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public UserGetModel Register(RegisterPostModel register)
        {
            User existing = dbcontext.Users.FirstOrDefault(u => u.Username == register.Username);
            if (existing != null)
            {
                return null;
            }

            dbcontext.Users.Add(new User
            {
                Email = register.Email,
                LastName = register.LastName,
                FirstName = register.FirstName,
                Password = ComputeSha256Hash(register.Password),
                Username = register.Username,
                //Role = UserRole.Regular,
                CreatedAt = DateTime.Now
            });
            dbcontext.SaveChanges();
            return Authenticate(register.Username, register.Password);
        }

        public User GetCurentUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            return dbcontext.Users.FirstOrDefault(u => u.Username == username);
        }


        public IEnumerable<UserGetModel> GetAll()
        {
            // return users without passwords
            return dbcontext.Users.Select(user => new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = null
            });
        }

        public User GetById(int id)
        {
            return dbcontext.Users
                .FirstOrDefault(u => u.Id == id);
        }

        public User Create(UserPostModel userModel)
        {
            User toAdd = UserPostModel.ToUser(userModel);
           
            HistoryUserRole history = new HistoryUserRole();
            history.Role.Title = RoleConstants.REGULAR;
            history.AllocatedAt = DateTime.Now;
            List<HistoryUserRole> list = new List<HistoryUserRole>
            {
                history
            };
            toAdd.Role = list;

            dbcontext.Users.Add(toAdd);
            dbcontext.SaveChanges();
            return toAdd;

        }

        public User Upsert(int id, UserPostModel userPostModel, User addedBy)
        {
            var existing = dbcontext.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
            String existingCurrentRole = getLatestHistoryUserRole(existing.Role);
            String addedByCurrentRole = getLatestHistoryUserRole(addedBy.Role);

            if (existing == null)
            {
                User toAdd = UserPostModel.ToUser(userPostModel);
                dbcontext.Users.Add(toAdd);
                dbcontext.SaveChanges();
                return toAdd;
            }

            User toUpdate = UserPostModel.ToUser(userPostModel);
            toUpdate.Password = existing.Password;
            toUpdate.CreatedAt = existing.CreatedAt;
            toUpdate.Id = id;

            if (userPostModel.UserRole.Equals(RoleConstants.ADMIN) && !addedByCurrentRole.Equals(RoleConstants.ADMIN))
            {
                return null;
            }
            else if (((!existingCurrentRole.Equals(RoleConstants.ADMIN) || (!existingCurrentRole.Equals(RoleConstants.USER_MANAGER)) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER))) ||
                (existingCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedBy.CreatedAt.AddMonths(6) <= DateTime.Now))
            {
                if(existing)
                toUpdate.Role = existing.Role;
                dbcontext.Users.Update(toUpdate);
                dbcontext.SaveChanges();
                return toUpdate;
            }
            else if (addedByCurrentRole.Equals(RoleConstants.ADMIN))
            {
                dbcontext.Users.Update(toUpdate);
                dbcontext.SaveChanges();
                return toUpdate;
            }           
            return null;
        }


        public User Delete(int id, User addedBy)
        {
            var existing = dbcontext.Users.FirstOrDefault(u => u.Id == id);
            String existingCurrentRole = getLatestHistoryUserRole(existing.Role);
            String addedByCurrentRole = getLatestHistoryUserRole(addedBy.Role);
            if (existing == null)
            {
                return null;
            }

            if (existingCurrentRole.Equals(RoleConstants.ADMIN) && !addedByCurrentRole.Equals(RoleConstants.ADMIN))
            {
                return null;
            }
            else if (((!existingCurrentRole.Equals(RoleConstants.ADMIN) || (!existingCurrentRole.Equals(RoleConstants.USER_MANAGER)) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER))) ||
                (existingCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedBy.CreatedAt.AddMonths(6) <= DateTime.Now))
            {
                dbcontext.Comments.RemoveRange(dbcontext.Comments.Where(u => u.Owner.Id == existing.Id));
                dbcontext.SaveChanges();
                dbcontext.Tasks.RemoveRange(dbcontext.Tasks.Where(u => u.Owner.Id == existing.Id));
                dbcontext.SaveChanges();

                dbcontext.Users.Remove(existing);
                dbcontext.SaveChanges();
                return existing;
            }
            else if (addedByCurrentRole.Equals(RoleConstants.ADMIN))
            {
                dbcontext.Comments.RemoveRange(dbcontext.Comments.Where(u => u.Owner.Id == existing.Id));
                dbcontext.SaveChanges();
                dbcontext.Tasks.RemoveRange(dbcontext.Tasks.Where(u => u.Owner.Id == existing.Id));
                dbcontext.SaveChanges();
                dbcontext.HistoryUserRoles.RemoveRange(dbcontext.HistoryUserRoles.Where(u => u.User.Id == existing.Id));
                dbcontext.SaveChanges();

                dbcontext.Users.Remove(existing);
                dbcontext.SaveChanges();
                return existing;
            }
            return null;          
        }

        private String getLatestHistoryUserRole(IEnumerable<HistoryUserRole> allHistory) {
            var latestHistoryUserRole = allHistory.OrderBy(x => x.AllocatedAt).FirstOrDefault();
            return latestHistoryUserRole.Role.Title;
        }
    }
}


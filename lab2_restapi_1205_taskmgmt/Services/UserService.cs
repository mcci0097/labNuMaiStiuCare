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

        UserGetModel GetById(int id);
        IEnumerable<HistoryGetModel> GetHistoryById(int id);
        //User Create(UserPostModel userModel);
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
                .Include(x => x.History)
                .ThenInclude(x => x.Role)
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
                    //new Claim(ClaimTypes.Role, user.Role.ToString())                    
                    new Claim(ClaimTypes.Role, getLatestHistoryUserRole(user.History).Role.Title)
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
        public UserGetModel Register(RegisterPostModel registerInfo)
        {
            User existing = dbcontext.Users.FirstOrDefault(u => u.Username == registerInfo.Username);
            if (existing != null)
            {
                return null;
            }

            User toBeAdded = new User
            { 
                Email = registerInfo.Email,
                LastName = registerInfo.LastName,
                FirstName = registerInfo.FirstName,
                Password = ComputeSha256Hash(registerInfo.Password),
                Username = registerInfo.Username,
                CreatedAt = DateTime.Now
            };
            dbcontext.Users.Add(toBeAdded);
            dbcontext.SaveChanges();
            dbcontext.Users.Attach(toBeAdded);
            

            Role role = new Role
            {
                Id = 1,         
                Title = RoleConstants.REGULAR
            };
            HistoryUserRole history = new HistoryUserRole
            {
                Role = role,
                AllocatedAt = DateTime.Now
            };
            List<HistoryUserRole> list = new List<HistoryUserRole>
            {
                history
            };

            dbcontext.Roles.Add(role);                        
            dbcontext.Roles.Attach(role);

            toBeAdded.History = list;

            dbcontext.SaveChanges();

            return Authenticate(registerInfo.Username, registerInfo.Password);
        }

        public User GetCurentUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            return dbcontext.Users.Include(x => x.History).ThenInclude(x => x.Role).FirstOrDefault(u => u.Username == username);
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

        public IEnumerable<HistoryGetModel> GetHistoryById(int id)
        {
            List<HistoryUserRole> histories = dbcontext.Users
                .Include(x => x.History)
                .ThenInclude(x => x.Role)
                .FirstOrDefault(u => u.Id == id).History;
            List<HistoryGetModel> returnList = new List<HistoryGetModel>();
            foreach (HistoryUserRole history in histories) {
                returnList.Add(HistoryGetModel.ToHistoryGetModel(history));
            }
            var list = returnList.OrderBy(x => x.AllocatedAt);
            return list;
        }

        public UserGetModel GetById(int id)
        {
            User user = dbcontext.Users
                .FirstOrDefault(u => u.Id == id);
            if (user == null) {
                return null;
            }
            return UserGetModel.FromUser(user);
        }

        public User Create(UserPostModel userModel)
        {
            User toBeAdded = new User
            {
                Email = userModel.Email,
                LastName = userModel.LastName,
                FirstName = userModel.FirstName,
                Password = ComputeSha256Hash("aaa"),
                Username = userModel.UserName,
            };
            dbcontext.Users.Add(toBeAdded);
            dbcontext.SaveChanges();
            dbcontext.Users.Attach(toBeAdded);

            Role role = new Role
            {
                Id = 1,
                Title = RoleConstants.REGULAR
            };
            HistoryUserRole history = new HistoryUserRole
            {
                Role = role,
                AllocatedAt = DateTime.Now
            };
            List<HistoryUserRole> list = new List<HistoryUserRole>
            {
                history
            };

            dbcontext.Roles.Add(role);
            dbcontext.Roles.Attach(role);

            toBeAdded.History = list;

            dbcontext.SaveChanges();

            return toBeAdded;
        }

        public User Upsert(int id, UserPostModel userPostModel, User addedBy)
        {
            var existing = dbcontext.Users.Include(x => x.History).ThenInclude(x => x.Role).AsNoTracking().FirstOrDefault(u => u.Id == id);
            //IEnumerable<HistoryUserRole> existing2 = dbcontext.HistoryUserRoles.Where(u => u.UserId == id);            
            String existingCurrentRole = getLatestHistoryUserRole(existing.History).Role.Title;
            String addedByCurrentRole = getLatestHistoryUserRole(addedBy.History).Role.Title;

            //String existingCurrentRole = getLatestHistoryUserRole(existing2).Role.Title;
            //String addedByCurrentRole = getLatestHistoryUserRole(addedBy.History).Role.Title;

            HistoryUserRole currentHistory = getLatestHistoryUserRole(existing.History);
            

            if (existing == null)
            {               
                //This is bullshit business wise, if there is no entity to be update, there should be a create logic... we dont have create logic for update logic
                User toAdd = Create(userPostModel);
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
            if (((!existingCurrentRole.Equals(RoleConstants.ADMIN) || (!existingCurrentRole.Equals(RoleConstants.USER_MANAGER)) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER))) ||
                (existingCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedBy.CreatedAt.AddMonths(6) <= DateTime.Now))
            {
                toUpdate.History = existing.History;
                dbcontext.Users.Update(toUpdate);
                dbcontext.SaveChanges();
                dbcontext.Users.Attach(toUpdate);

                if (existingCurrentRole != userPostModel.UserRole) {
                    IEnumerable<Role> allRoles = dbcontext.Roles;
                    List<String> list = new List<string>();
                    foreach (Role role in allRoles) {
                        list.Add(role.Title);
                    }
                    if (list.Contains(userPostModel.UserRole))
                    {
                        Role role = searchForRoleByTitle(userPostModel.UserRole);
                        HistoryUserRole history = new HistoryUserRole
                        {
                            Role = role,
                            AllocatedAt = DateTime.Now
                        };
                        currentHistory.RemovedAt = DateTime.Now;

                        dbcontext.Roles.Attach(role);
                        toUpdate.History.Add(history);
                        dbcontext.SaveChanges();
                    }
                    else {
                        return null;
                    }                
                }

                return toUpdate;
            }
            if (addedByCurrentRole.Equals(RoleConstants.ADMIN))
            {
                dbcontext.Users.Update(toUpdate);
                dbcontext.SaveChanges();

                if (existingCurrentRole != userPostModel.UserRole)
                {
                    IEnumerable<Role> allRoles = dbcontext.Roles;
                    List<String> list = new List<string>();
                    foreach (Role role in allRoles)
                    {
                        list.Add(role.Title);
                    }
                    if (list.Contains(userPostModel.UserRole))
                    {
                        Role role = searchForRoleByTitle(userPostModel.UserRole);
                        HistoryUserRole history = new HistoryUserRole
                        {
                            Role = role,
                            AllocatedAt = DateTime.Now
                        };
                        currentHistory.RemovedAt = DateTime.Now;

                        dbcontext.Roles.Attach(role);
                        toUpdate.History.Add(history);                        
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        return null;
                    }
                }

                return toUpdate;
            }
            return null;
        }


        public User Delete(int id, User addedBy)
        {
            var existing = dbcontext.Users.Include(x => x.History).ThenInclude(x => x.Role).FirstOrDefault(u => u.Id == id);
            String existingCurrentRole = getLatestHistoryUserRole(existing.History).Role.Title;
            String addedByCurrentRole = getLatestHistoryUserRole(addedBy.History).Role.Title;
            if (existing == null)
            {
                return null;
            }

            if (existingCurrentRole.Equals(RoleConstants.ADMIN) && !addedByCurrentRole.Equals(RoleConstants.ADMIN))
            {
                return null;
            }
            if (((!existingCurrentRole.Equals(RoleConstants.ADMIN) || (!existingCurrentRole.Equals(RoleConstants.USER_MANAGER)) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER))) ||
                (existingCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedByCurrentRole.Equals(RoleConstants.USER_MANAGER) && addedBy.CreatedAt.AddMonths(6) <= DateTime.Now))
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

        private HistoryUserRole getLatestHistoryUserRole(IEnumerable<HistoryUserRole> allHistory)
        {
            //TODO see what is needed
            var latestHistoryUserRole = allHistory.OrderByDescending(x => x.AllocatedAt).FirstOrDefault();
            //var latestHistoryUserRole = allHistory.OrderByDescending(x => x.AllocatedAt).FirstOrDefault();
            if (latestHistoryUserRole.RemovedAt == null)
            {
                return latestHistoryUserRole;
            }
            //TODO if we erase from db directly, the RemovedAt will not change to null and the user will have no current role
            return null;
        }

        private Role searchForRoleByTitle(String title)
        {
            IEnumerable<Role> roles = dbcontext.Roles;
            foreach (Role role in roles) {
                if (role.Title.Equals(title))
                {
                    return role;
                }
            }
            return null;
        }

        public void setRoleAdmin(User user)
        {
            Role admin = new Role
            {
                Id = 3,
                Title = RoleConstants.ADMIN               
            };
            HistoryUserRole history = new HistoryUserRole
            {
                Role = admin,
                AllocatedAt = DateTime.Now,
            };
            //getLatestHistoryUserRole(user.History).RemovedAt = DateTime.Now;            
            user.History.Add(history);
        }

    }
}


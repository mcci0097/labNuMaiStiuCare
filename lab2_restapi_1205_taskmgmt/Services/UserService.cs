using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.ViewModels;
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
                    new Claim(ClaimTypes.Name, user.Username.ToString())
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
                Username = register.Username
            });
            dbcontext.SaveChanges();
            return Authenticate(register.Username, register.Password); 
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
    }
}


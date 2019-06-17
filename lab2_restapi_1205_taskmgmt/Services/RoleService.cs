using lab2_restapi_1205_taskmgmt.Models;
using lab2_restapi_1205_taskmgmt.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.Services
{
    public interface IRoleService
    {
        PaginatedList<RoleGetModel> GetAll(int page, DateTime? from = null, DateTime? to = null);

        Role Create(RolePostModel role);

        Role Upsert(int id, RolePostModel rolePostModel);

        Role Delete(int id);

        Role GetById(int id);
    }

    public class RoleService : IRoleService
    {
        private TasksDbContext dbcontext;

        private readonly AppSettings appSettings;

        public RoleService(TasksDbContext context, IOptions<AppSettings> appSettings)
        {
            this.dbcontext = context;
            this.appSettings = appSettings.Value;
        }

        public PaginatedList<RoleGetModel> GetAll(int page, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Role> result = dbcontext
                            .Roles
                            .OrderBy(t => t.Id);
            PaginatedList<RoleGetModel> paginatedList = new PaginatedList<RoleGetModel>();
            paginatedList.CurrentPage = page;

            //if there are more includes use thenInclude


            paginatedList.NumberOfPages = (result.Count() - 1) / PaginatedList<TaskGetModel>.EntriesPerPage + 1;
            result = result
                .Skip((page - 1) * PaginatedList<RoleGetModel>.EntriesPerPage)
                .Take(PaginatedList<RoleGetModel>.EntriesPerPage);
            paginatedList.Entries = result.Select(t => RoleGetModel.FromRole(t)).ToList();

            return paginatedList;
        }

        public Role GetById(int id)
        {
            return dbcontext.Roles
                .FirstOrDefault(u => u.Id == id);
        }

        public Role Create(RolePostModel roleModel)
        {
            Role toAdd = RolePostModel.ToRole(roleModel);

            dbcontext.Roles.Add(toAdd);
            dbcontext.SaveChanges();
            return toAdd;            
        }

        public Role Upsert(int id, RolePostModel rolePostModel)
        {
            var existing = dbcontext.Roles.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                Role toAdd = RolePostModel.ToRole(rolePostModel);
                dbcontext.Roles.Add(toAdd);
                dbcontext.SaveChanges();
                return toAdd;
            }

            Role toUpdate = RolePostModel.ToRole(rolePostModel);
            toUpdate.Id = id;
            dbcontext.Roles.Update(toUpdate);
            dbcontext.SaveChanges();
            return toUpdate;           
        }


        public Role Delete(int id)
        {
            var existing = dbcontext.Roles.FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                return null;
            }
            dbcontext.Roles.Remove(existing);
            dbcontext.SaveChanges();
            return existing;            
        }
    }
}

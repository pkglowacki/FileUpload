using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.EntityFramework;
using AspUploadSample.Models;
using System.Threading.Tasks;

namespace AspUploadSample.Controllers
{
    public abstract class BaseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected IPagedList<FileModel> GetPagedFiles(int? page)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetStuffFromDatabase();

            // page the list
            const int pageSize = 10;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        protected IQueryable<FileModel> GetStuffFromDatabase()
        {
            var files = from s in db.Files
                             select s;
            files = files.OrderBy(s => s.Id);
            return files;
        }


        protected IPagedList<ApplicationUser> GetPagedUsers(int? page)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = GetUsersFromDatabase();

            // page the list
            const int pageSize = 10;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }

        protected IQueryable<ApplicationUser> GetUsersFromDatabase()
        {
            var users = from s in db.Users
                        select s;
            users = users.OrderBy(s => s.Id);
            return users;
        }
    }
}
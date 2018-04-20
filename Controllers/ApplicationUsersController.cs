using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using AspUploadSample.Models;
using AspUploadSample;
using PagedList.EntityFramework;
using System.Data.Entity.Infrastructure;
using AspUploadSample.Controllers;

namespace SinglePageAppExample.Controllers
{
    [Authorize(Roles="Administrator")]
    public class ApplicationUsersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    
        // GET: ApplicationUsers
        public async Task<ActionResult> Index(int? page, string searchString, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var listPaged = GetPagedUsers(page);

           // List<ApplicationUser> list = new List<ApplicationUser>();
            IQueryable<ApplicationUser> list = from u in db.Users select u;
            list = list.Where(u => u.UserName != "Administrator");

            //foreach (var user in db.Users)
            //{
            //    user.userRoles = UserManager.GetRoles(user.Id);
            //    if (user.UserName != "Administrator")
            //    {
            //        list.Add(user);
            //    }
            //}

            if (!String.IsNullOrEmpty(searchString))
            {
               // list = list.FindAll(u => u.UserName.Contains(searchString) && u.UserName != "Administrator");
                list = list.Where(s => s.UserName.Contains(searchString));
            }

            list = list.OrderBy(s => s.Id);

            int pageNumber = (page ?? 1);
            return View(await list.ToPagedListAsync(listPaged.PageNumber, listPaged.PageSize));
        }

        // GET: ApplicationUsers/Details/5
        public async Task<ActionResult> Details(string id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser applicationUser = await UserManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            //var pagedList = GetPagedFiles(page);

            //if (applicationUser.UserName == "Administrator")
             //   return RedirectToAction("Index");
            applicationUser.userRoles = await UserManager.GetRolesAsync(id);

             //IQueryable<FileModel> files = from f in db.Files select f;

             //files = files.Where(x => x.User_Id == id).OrderBy(x => x.Id);
            //files = files.OrderBy(f => f.Id);

            //var pageNumber = page ?? 1;
            //ViewBag.onePageOfFiles = await files.ToPagedListAsync(pagedList.PageNumber, pagedList.PageSize);

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Email,PasswordHash,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser applicationUser = db.Users.Find(id);

            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            
            if (applicationUser.UserName == "Administrator")
                return RedirectToAction("Index");

            ViewBag.userRoles = UserManager.GetRoles(id).First();
            ViewBag.rolesList = new SelectList(db.Roles, "Name", "Name", ViewBag.userRoles);

            return View(applicationUser);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Exclude = "PasswordHash,SecurityStamp,Id")]string id, string selectedRole)
        {
            string[] fieldsToBind = new string[] { "UserName","Email","RowVersion"};

            var userToUpdate = db.Users.Find(id);

            if (userToUpdate == null)
            {
                ApplicationUser deletedUser = new ApplicationUser();
                TryUpdateModel(deletedUser, fieldsToBind);
                ModelState.AddModelError(string.Empty, "Unable to save changes. The user was deleted by admin.");
                return View(deletedUser);
            }
            ViewBag.userRoles = UserManager.GetRoles(userToUpdate.Id).First();

            await UserManager.RemoveFromRoleAsync(userToUpdate.Id, "User");
            await UserManager.RemoveFromRoleAsync(userToUpdate.Id, "Administrator");
            await UserManager.AddToRolesAsync(userToUpdate.Id, selectedRole);
            await PassReset(id);

            db.Entry(userToUpdate).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser.UserName == "Administrator")
                return RedirectToAction("Index");
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            var list = from f in db.Files where f.User_Id == applicationUser.Id select f;

            foreach (var item in list)
            {
                item.User_Id = null;
            }

            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> PassReset(string id)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);

            string v = UserManager.FindById(id).Id;

            // String userId = db.Users.Find(UserManager.FindById(id)).Id; //"<YourLogicAssignsRequestedUserId>";
            //  String newPassword = db.Users.Find(UserManager.FindById(id)).UserName + "@123"; //"<PasswordAsTypedByUser>";
            string newPassword = UserManager.FindById(id).UserName + "@123";
            String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);

            ApplicationUser cUser = await store.FindByIdAsync(v); //userId

            await store.SetPasswordHashAsync(cUser, hashedNewPassword);
            await store.UpdateAsync(cUser);

            return View(cUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

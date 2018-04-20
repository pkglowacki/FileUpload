using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspUploadSample.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Text;
using PagedList.EntityFramework;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace AspUploadSample.Controllers
{
    [Authorize]
    public class FilesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Files
        [HttpGet]
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

            var listPaged = GetPagedFiles(page);

            IQueryable<FileModel> files = from s in db.Files select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                files = files.Where(s => s.FileName.Contains(searchString));
            }

            files = files.OrderBy(s => s.FileType);
            int pageNumber = (page ?? 1);

            return View(await files.ToPagedListAsync(listPaged.PageNumber, listPaged.PageSize));
        }

        public ActionResult SynchronizeDatabase()
        {
            int i = 0;
            foreach (var file in db.Files)
            {
                if (!System.IO.File.Exists(file.FilePath))
                {
                    i++;
                    db.Files.Remove(file);
                }
            }
            db.SaveChanges();
            TempData["AlertMessage"] = String.Format("Database synchronized, {0} files deleted", i);
            return RedirectToAction("Index");
        }

        public ActionResult SynchronizeFolder()
        {
            DirectoryInfo FileDirectory = new DirectoryInfo(@"C:\Users\admin\Documents\Visual Studio 2013\Projects\AspUploadSample\AspUploadSample\UploadFolder");

            int i = 0;
            var FileNameWithoutExtension = "";
            foreach (var file in FileDirectory.GetFiles())
            {
                FileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
                if (db.Files.Where(f => f.FileName == FileNameWithoutExtension).Count() > 0)
                    continue;
                else
                {
                    System.IO.File.Delete(file.FullName);
                    i++;
                }
            }
            TempData["AlertMessage"] = String.Format("Folder synchronized, number of deleted files: {0}", i);
            return RedirectToAction("Index");
        }

        // GET: Files/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileModel file = await db.Files.FindAsync(id);

            if (file == null)
            {
                return HttpNotFound();
            }

            switch (file.FileType)
            {
                case ".txt":
                    using (StreamReader reader = new StreamReader(file.FilePath))
                    {
                        ViewBag.FileContent = await reader.ReadToEndAsync();
                        reader.Close();
                    }
                    break;
                
                case ".cs":
                    using (StreamReader reader = new StreamReader(file.FilePath))
                    {
                        ViewBag.FileContent = await reader.ReadToEndAsync();
                        reader.Close();
                    }
                    break;
                
                case ".dll":
                    using (StreamReader reader = new StreamReader(file.FilePath))
                    {
                        ViewBag.FileContent = await reader.ReadToEndAsync();
                        reader.Close();
                    }
                    break;
                
                default:
                    ViewBag.FileContent = HttpUtility.UrlPathEncode(string.Concat(@"~\UploadFolder\", file.FileName, file.FileType));
                    break;

            }
            //if (file.FileType == ".txt")
            //{
            //    using (StreamReader reader = new StreamReader(file.FilePath))
            //    {
            //        ViewBag.FileContent = await reader.ReadToEndAsync();
            //        reader.Close();
            //    }
            //}
            //else
            //{
            //    ViewBag.FileContent = HttpUtility.UrlPathEncode(string.Concat(@"~\UploadFolder\", file.FileName, file.FileType));
            //}
            return PartialView(file);
        }

        // GET: Files/Upload
        public ActionResult Upload()
        {
            return View();
        }

        // POST: Files/Upload
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Upload([Bind(Include = "DataType,FilePath,FileName,User_Id")] FileModel ModelFile, HttpPostedFileBase[] filesToUpload)
        {
            if (filesToUpload[0] == null)
            {
                TempData["AlertMessage"] = "Choose file";
                return RedirectToAction("Index");
            }

            foreach (HttpPostedFileBase fileToUpload in filesToUpload)
            {
                try
                {
                    if (fileToUpload.ContentLength == 0) { TempData["AlertMessage"] = "Empty files skipped";
                                                           continue; }
                    else
                    {
                        if (fileToUpload.ContentLength > 0 && filesToUpload != null)
                        {
                            string _FileName = Path.GetFileName(fileToUpload.FileName);
                            string _path = Path.Combine(Server.MapPath("~/UploadFolder"), _FileName);

                            ModelFile.FileType = Path.GetExtension(fileToUpload.FileName);
                            ModelFile.FilePath = _path;
                            ModelFile.FileName = Path.GetFileNameWithoutExtension(fileToUpload.FileName);
                            ModelFile.UploadFolder = Server.MapPath("~/UploadFolder");
                            ModelFile.User_Id = HttpContext.User.Identity.GetUserId();
                            
                            db.Files.Add(ModelFile);
                            fileToUpload.SaveAs(_path);
                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    TempData["AlertMessage"] = String.Format("File with name: {0} already exists in database", fileToUpload.FileName);
                    continue;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(string FileContent, string FileName)
        {
            FileModel file = new FileModel();

            if (FileContent.Length > 0 && FileName.Length > 0)
            {
                file.FileType = ".txt";
                file.FileName = FileName;
                file.UploadFolder = Server.MapPath("~/UploadFolder");
                file.User_Id = HttpContext.User.Identity.GetUserId();
                file.FilePath = Path.Combine(Server.MapPath("~/UploadFolder"), FileName+".txt");
                System.IO.File.WriteAllText(Path.Combine(Server.MapPath("~/UploadFolder"), FileName+".txt"), FileContent);
                
                db.Files.Add(file);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else TempData["AlertMessage"] = "File name or content is empty ";
            return RedirectToAction("Index");
        }

        // GET: Files/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileModel file = await db.Files.FindAsync(id);

            if (file == null)
            {
                return HttpNotFound();
            }

            if (file.FileType != ".mp3" && file.FileType != ".wav")
            {
                using (StreamReader reader = new StreamReader(file.FilePath))
                {
                    ViewBag.FileContent = await reader.ReadToEndAsync();
                    reader.Close();
                }
            }
            return PartialView(file);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( int? id, string FileContent, byte[] rowVersion, ApplicationUser appUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string[] fieldsToBind = new string[] { "FileName", "LastEditor", "LastEditDate", "RowVersion" };

            var fileToUpdate = await db.Files.FindAsync(id);
            if (fileToUpdate == null)
            {
                FileModel deletedFile = new FileModel();
                TryUpdateModel(deletedFile,fieldsToBind);
                ModelState.AddModelError(string.Empty, "Unable to save changes. The file was deleted by another user.");
                return View(deletedFile);
            }

            if ((fileToUpdate.FileType != ".mp3") && (fileToUpdate.FileType != ".wav"))
            {
                using (StreamWriter writer = new StreamWriter(fileToUpdate.FilePath))
                {
                    writer.Write(FileContent);
                    writer.Close();
                }
            }

            if (TryUpdateModel(fileToUpdate, fieldsToBind))
            {
                try
                {
                    fileToUpdate.LastEditDate = DateTime.Now;
                    fileToUpdate.LastEditor = HttpContext.User.Identity.GetUserName();

                    db.Entry(fileToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (FileModel)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The file was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (FileModel)databaseEntry.ToObject();

                        if (databaseValues.FileName != clientValues.FileName)
                            ModelState.AddModelError("FileName", "Current value: " + databaseValues.FileName);
                        if (databaseValues.LastEditor != clientValues.LastEditor)
                            ModelState.AddModelError("LastEditor", "Current value: " + databaseValues.LastEditor);
                        if (databaseValues.LastEditDate != clientValues.LastEditDate)
                            ModelState.AddModelError("LastEditDate", "Current value: " + String.Format("{0:d}", databaseValues.LastEditDate));
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                        fileToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(fileToUpdate);
        }

        // GET: Files/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            FileModel file = await db.Files.FindAsync(id);

            if (file == null)
            {
                return HttpNotFound();
            }
            return PartialView(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            FileModel file = db.Files.Find(id);
            
            if (System.IO.File.Exists(file.FilePath))
            {
                System.IO.File.Delete(file.FilePath);
            }
            
            db.Files.Remove(file);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
namespace AspUploadSample.Migrations
{
    using AspUploadSample.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AspUploadSample.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AspUploadSample.Models.ApplicationDbContext context)
        {
            SeedRoles(context);
            SeedAdmin(context);
        }

        private void SeedRoles(Models.ApplicationDbContext context)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!RoleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                RoleManager.Create(role);
            }


            if (!RoleManager.RoleExists("Administrator"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Administrator";
                RoleManager.Create(role);
            }

        }

        private void SeedAdmin(Models.ApplicationDbContext context)
        {
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            if (!context.Users.Any(u => u.UserName == "Administrator"))
            {
                var user = new ApplicationUser { UserName = "Administrator", Email = "ad@ad.com" };
                var adminresult = manager.Create(user, "1qaz!QAZ");

                if (adminresult.Succeeded)
                {
                    manager.AddToRole(user.Id, "Administrator");
                }
            }
        }
    }
}

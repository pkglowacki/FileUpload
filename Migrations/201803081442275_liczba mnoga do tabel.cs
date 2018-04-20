namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liczbamnogadotabel : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.User", newName: "Users");
            RenameTable(name: "dbo.UserRole", newName: "UserRoles");
            RenameTable(name: "dbo.Role", newName: "Roles");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Roles", newName: "Role");
            RenameTable(name: "dbo.UserRoles", newName: "UserRole");
            RenameTable(name: "dbo.Users", newName: "User");
        }
    }
}

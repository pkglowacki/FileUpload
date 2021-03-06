namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unrequiredfilemodel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "FilePath", c => c.String());
            AlterColumn("dbo.Files", "UploadFolder", c => c.String());
            AlterColumn("dbo.Files", "DataType", c => c.String());
            AlterColumn("dbo.Files", "FileName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "FileName", c => c.String(nullable: false));
            AlterColumn("dbo.Files", "DataType", c => c.String(nullable: false));
            AlterColumn("dbo.Files", "UploadFolder", c => c.String(nullable: false));
            AlterColumn("dbo.Files", "FilePath", c => c.String(nullable: false));
        }
    }
}

namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datacontentaddedtofilemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "DataContent", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "DataContent");
        }
    }
}

namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datacontentfromfilemodelremoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Files", "DataContent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Files", "DataContent", c => c.String());
        }
    }
}

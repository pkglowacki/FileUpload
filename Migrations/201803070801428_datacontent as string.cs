namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datacontentasstring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "DataContent", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "DataContent", c => c.Binary());
        }
    }
}

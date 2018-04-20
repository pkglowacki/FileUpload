namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddatestring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "AddDate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "AddDate", c => c.DateTime());
        }
    }
}

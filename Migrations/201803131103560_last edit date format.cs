namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lasteditdateformat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "LastEditDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "LastEditDate", c => c.DateTime(nullable: false));
        }
    }
}

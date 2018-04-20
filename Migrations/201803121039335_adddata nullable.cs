namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddatanullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "AddDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "AddDate", c => c.DateTime(nullable: false));
        }
    }
}

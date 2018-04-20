namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Timestamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "RowVersion");
        }
    }
}

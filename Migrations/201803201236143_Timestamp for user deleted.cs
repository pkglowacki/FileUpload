namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Timestampforuserdeleted : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
    }
}

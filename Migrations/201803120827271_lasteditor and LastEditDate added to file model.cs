namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lasteditorandLastEditDateaddedtofilemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "LastEditor", c => c.String());
            AddColumn("dbo.Files", "LastEditDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "LastEditDate");
            DropColumn("dbo.Files", "LastEditor");
        }
    }
}

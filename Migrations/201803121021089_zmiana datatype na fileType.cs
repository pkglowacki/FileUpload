namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zmianadatatypenafileType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "FileType", c => c.String());
            DropColumn("dbo.Files", "DataType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Files", "DataType", c => c.String());
            DropColumn("dbo.Files", "FileType");
        }
    }
}

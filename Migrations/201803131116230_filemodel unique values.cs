namespace AspUploadSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class filemodeluniquevalues : DbMigration
    {
        public override void Up()
        {
           // CreateIndex("dbo.Files", "FilePath", unique: true, name: "FilePathIndex");
            CreateIndex("dbo.Files", "FileName", unique: true, name: "FileNameIndex");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Files", "FileNameIndex");
          //  DropIndex("dbo.Files", "FilePathIndex");
        }
    }
}

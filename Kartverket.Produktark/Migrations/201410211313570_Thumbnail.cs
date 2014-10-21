namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Thumbnail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "Thumbnail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "Thumbnail");
        }
    }
}

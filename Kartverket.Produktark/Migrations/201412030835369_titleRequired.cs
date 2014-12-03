namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class titleRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductSheets", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSheets", "Title", c => c.String());
        }
    }
}

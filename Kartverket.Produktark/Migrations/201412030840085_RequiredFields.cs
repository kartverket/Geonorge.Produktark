namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductSheets", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.ProductSheets", "Purpose", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSheets", "Purpose", c => c.String());
            AlterColumn("dbo.ProductSheets", "Description", c => c.String());
        }
    }
}

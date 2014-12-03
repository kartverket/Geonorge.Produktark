namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFields : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.ProductSheets SET Description = '' WHERE Description IS NULL");
            Sql("UPDATE dbo.ProductSheets SET Purpose = '' WHERE Purpose IS NULL"); 
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

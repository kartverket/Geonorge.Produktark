namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurposeNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductSheets", "Purpose", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSheets", "Purpose", c => c.String(nullable: false));
        }
    }
}

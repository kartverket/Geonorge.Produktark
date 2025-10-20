namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResolutionDistance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "ResolutionDistance", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "ResolutionDistance");
        }
    }
}

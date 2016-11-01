namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDistributionFormats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "DistributionFormats", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "DistributionFormats");
        }
    }
}

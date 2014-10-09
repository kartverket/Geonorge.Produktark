namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdditionalFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "KeywordsPlace", c => c.String());
            AddColumn("dbo.ProductSheets", "PrecisionInMeters", c => c.String());
            AddColumn("dbo.ProductSheets", "CoverageArea", c => c.String());
            AddColumn("dbo.ProductSheets", "Projections", c => c.String());
            AddColumn("dbo.ProductSheets", "ServiceDetails", c => c.String());
            AddColumn("dbo.ProductSheets", "ListOfFeatureTypes", c => c.String());
            AddColumn("dbo.ProductSheets", "ListOfAttributes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "ListOfAttributes");
            DropColumn("dbo.ProductSheets", "ListOfFeatureTypes");
            DropColumn("dbo.ProductSheets", "ServiceDetails");
            DropColumn("dbo.ProductSheets", "Projections");
            DropColumn("dbo.ProductSheets", "CoverageArea");
            DropColumn("dbo.ProductSheets", "PrecisionInMeters");
            DropColumn("dbo.ProductSheets", "KeywordsPlace");
        }
    }
}

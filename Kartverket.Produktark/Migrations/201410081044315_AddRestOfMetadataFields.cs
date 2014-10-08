namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRestOfMetadataFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "ContactMetadata_Name", c => c.String());
            AddColumn("dbo.ProductSheets", "ContactMetadata_Email", c => c.String());
            AddColumn("dbo.ProductSheets", "ContactPublisher_Name", c => c.String());
            AddColumn("dbo.ProductSheets", "ContactPublisher_Email", c => c.String());
            AddColumn("dbo.ProductSheets", "ContactOwner_Name", c => c.String());
            AddColumn("dbo.ProductSheets", "ContactOwner_Email", c => c.String());
            AddColumn("dbo.ProductSheets", "ResolutionScale", c => c.String());
            AddColumn("dbo.ProductSheets", "ProcessHistory", c => c.String());
            AddColumn("dbo.ProductSheets", "MaintenanceFrequency", c => c.String());
            AddColumn("dbo.ProductSheets", "Status", c => c.String());
            AddColumn("dbo.ProductSheets", "DistributionFormatName", c => c.String());
            AddColumn("dbo.ProductSheets", "DistributionFormatVersion", c => c.String());
            AddColumn("dbo.ProductSheets", "AccessConstraints", c => c.String());
            AddColumn("dbo.ProductSheets", "LegendDescriptionUrl", c => c.String());
            AddColumn("dbo.ProductSheets", "ProductPageUrl", c => c.String());
            AddColumn("dbo.ProductSheets", "ProductSpecificationUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "ProductSpecificationUrl");
            DropColumn("dbo.ProductSheets", "ProductPageUrl");
            DropColumn("dbo.ProductSheets", "LegendDescriptionUrl");
            DropColumn("dbo.ProductSheets", "AccessConstraints");
            DropColumn("dbo.ProductSheets", "DistributionFormatVersion");
            DropColumn("dbo.ProductSheets", "DistributionFormatName");
            DropColumn("dbo.ProductSheets", "Status");
            DropColumn("dbo.ProductSheets", "MaintenanceFrequency");
            DropColumn("dbo.ProductSheets", "ProcessHistory");
            DropColumn("dbo.ProductSheets", "ResolutionScale");
            DropColumn("dbo.ProductSheets", "ContactOwner_Email");
            DropColumn("dbo.ProductSheets", "ContactOwner_Name");
            DropColumn("dbo.ProductSheets", "ContactPublisher_Email");
            DropColumn("dbo.ProductSheets", "ContactPublisher_Name");
            DropColumn("dbo.ProductSheets", "ContactMetadata_Email");
            DropColumn("dbo.ProductSheets", "ContactMetadata_Name");
        }
    }
}

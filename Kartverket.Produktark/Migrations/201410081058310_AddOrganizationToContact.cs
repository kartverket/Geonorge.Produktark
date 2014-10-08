namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganizationToContact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "ContactMetadata_Organization", c => c.String());
            AddColumn("dbo.ProductSheets", "ContactPublisher_Organization", c => c.String());
            AddColumn("dbo.ProductSheets", "ContactOwner_Organization", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "ContactOwner_Organization");
            DropColumn("dbo.ProductSheets", "ContactPublisher_Organization");
            DropColumn("dbo.ProductSheets", "ContactMetadata_Organization");
        }
    }
}

namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFields1 : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.ProductSheets SET ContactMetadata_Email = '' WHERE ContactMetadata_Email IS NULL");
            Sql("UPDATE dbo.ProductSheets SET ContactMetadata_Organization = '' WHERE ContactMetadata_Organization IS NULL");
            Sql("UPDATE dbo.ProductSheets SET ContactPublisher_Email = '' WHERE ContactPublisher_Email IS NULL");
            Sql("UPDATE dbo.ProductSheets SET ContactPublisher_Organization = '' WHERE ContactPublisher_Organization IS NULL");
            Sql("UPDATE dbo.ProductSheets SET ContactOwner_Email = '' WHERE ContactOwner_Email IS NULL");
            Sql("UPDATE dbo.ProductSheets SET ContactOwner_Organization = '' WHERE ContactOwner_Organization IS NULL");

            AlterColumn("dbo.ProductSheets", "ContactMetadata_Email", c => c.String(nullable: false));
            AlterColumn("dbo.ProductSheets", "ContactMetadata_Organization", c => c.String(nullable: false));
            AlterColumn("dbo.ProductSheets", "ContactPublisher_Email", c => c.String(nullable: false));
            AlterColumn("dbo.ProductSheets", "ContactPublisher_Organization", c => c.String(nullable: false));
            AlterColumn("dbo.ProductSheets", "ContactOwner_Email", c => c.String(nullable: false));
            AlterColumn("dbo.ProductSheets", "ContactOwner_Organization", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSheets", "ContactOwner_Organization", c => c.String());
            AlterColumn("dbo.ProductSheets", "ContactOwner_Email", c => c.String());
            AlterColumn("dbo.ProductSheets", "ContactPublisher_Organization", c => c.String());
            AlterColumn("dbo.ProductSheets", "ContactPublisher_Email", c => c.String());
            AlterColumn("dbo.ProductSheets", "ContactMetadata_Organization", c => c.String());
            AlterColumn("dbo.ProductSheets", "ContactMetadata_Email", c => c.String());
        }
    }
}

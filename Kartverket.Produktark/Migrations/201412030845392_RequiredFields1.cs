namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFields1 : DbMigration
    {
        public override void Up()
        {
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

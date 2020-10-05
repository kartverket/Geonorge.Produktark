namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLicense : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "UseConstraintsLicenseLinkText", c => c.String());
            AddColumn("dbo.ProductSheets", "UseConstraintsLicenseLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "UseConstraintsLicenseLink");
            DropColumn("dbo.ProductSheets", "UseConstraintsLicenseLinkText");
        }
    }
}

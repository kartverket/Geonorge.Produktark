namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetatadataFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductSheets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Uuid = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        SupplementalDescription = c.String(),
                        Purpose = c.String(),
                        SpecificUsage = c.String(),
                        UseLimitations = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductSheets");
        }
    }
}

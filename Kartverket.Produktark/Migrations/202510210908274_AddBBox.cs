namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBBox : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSheets", "BBoxNorth", c => c.String());
            AddColumn("dbo.ProductSheets", "BBoxSouth", c => c.String());
            AddColumn("dbo.ProductSheets", "BBoxEast", c => c.String());
            AddColumn("dbo.ProductSheets", "BBoxWest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSheets", "BBoxWest");
            DropColumn("dbo.ProductSheets", "BBoxEast");
            DropColumn("dbo.ProductSheets", "BBoxSouth");
            DropColumn("dbo.ProductSheets", "BBoxNorth");
        }
    }
}

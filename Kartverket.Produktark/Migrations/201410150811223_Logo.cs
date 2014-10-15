namespace Kartverket.Produktark.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Logo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Organization = c.String(),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Logos");
        }
    }
}

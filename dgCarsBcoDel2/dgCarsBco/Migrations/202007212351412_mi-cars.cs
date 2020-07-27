namespace dgCarsBco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class micars : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Model = c.String(),
                        Manufacturer = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Cars");
        }
    }
}

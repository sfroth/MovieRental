namespace MovieRental.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountMovie",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RentalDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        Account_ID = c.Int(nullable: false),
                        Movie_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Account", t => t.Account_ID, cascadeDelete: true)
                .ForeignKey("dbo.Movie", t => t.Movie_ID, cascadeDelete: true)
                .Index(t => t.Account_ID)
                .Index(t => t.Movie_ID);
            
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Movie",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        ImdbId = c.String(),
                        ReleaseDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StreetAddress1 = c.String(nullable: false),
                        StreetAddress2 = c.String(),
                        City = c.String(nullable: false),
                        StateProvince = c.String(),
                        Country = c.String(nullable: false),
                        PostalCode = c.String(),
                        Latitude = c.String(),
                        Longitude = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.KioskMovie",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Stock = c.Int(nullable: false),
                        Kiosk_ID = c.Int(nullable: false),
                        Movie_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Kiosk", t => t.Kiosk_ID, cascadeDelete: true)
                .ForeignKey("dbo.Movie", t => t.Movie_ID, cascadeDelete: true)
                .Index(t => t.Kiosk_ID)
                .Index(t => t.Movie_ID);
            
            CreateTable(
                "dbo.Kiosk",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Address_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Address", t => t.Address_ID, cascadeDelete: true)
                .Index(t => t.Address_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KioskMovie", "Movie_ID", "dbo.Movie");
            DropForeignKey("dbo.KioskMovie", "Kiosk_ID", "dbo.Kiosk");
            DropForeignKey("dbo.Kiosk", "Address_ID", "dbo.Address");
            DropForeignKey("dbo.AccountMovie", "Movie_ID", "dbo.Movie");
            DropForeignKey("dbo.AccountMovie", "Account_ID", "dbo.Account");
            DropIndex("dbo.Kiosk", new[] { "Address_ID" });
            DropIndex("dbo.KioskMovie", new[] { "Movie_ID" });
            DropIndex("dbo.KioskMovie", new[] { "Kiosk_ID" });
            DropIndex("dbo.AccountMovie", new[] { "Movie_ID" });
            DropIndex("dbo.AccountMovie", new[] { "Account_ID" });
            DropTable("dbo.Kiosk");
            DropTable("dbo.KioskMovie");
            DropTable("dbo.Address");
            DropTable("dbo.Movie");
            DropTable("dbo.Account");
            DropTable("dbo.AccountMovie");
        }
    }
}

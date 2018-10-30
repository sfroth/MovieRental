namespace MovieRental.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class GeoLocationRentalReturnNullable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Address", "Location", c => c.Geography());
            AlterColumn("dbo.AccountMovie", "ReturnDate", c => c.DateTime());
            DropColumn("dbo.Address", "Latitude");
            DropColumn("dbo.Address", "Longitude");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Address", "Longitude", c => c.String());
            AddColumn("dbo.Address", "Latitude", c => c.String());
            AlterColumn("dbo.AccountMovie", "ReturnDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Address", "Location");
        }
    }
}

namespace MovieRental.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "Active");
        }
    }
}

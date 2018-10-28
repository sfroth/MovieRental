namespace MovieRental.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Auth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "Username", c => c.String(nullable: false, maxLength: 40));
            AddColumn("dbo.Account", "Password", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Account", "Role", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "Role");
            DropColumn("dbo.Account", "Password");
            DropColumn("dbo.Account", "Username");
        }
    }
}

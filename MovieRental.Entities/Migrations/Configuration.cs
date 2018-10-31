namespace MovieRental.Entities.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MovieRental.Entities.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MovieRental.Entities.DataContext";
        }

        protected override void Seed(MovieRental.Entities.DataContext context)
        {
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data.
			context.Accounts.AddOrUpdate(p => p.Username, new Models.Account { Username = "joe", Password = "user", UserRole = "Admin", Active = true });
        }
    }
}

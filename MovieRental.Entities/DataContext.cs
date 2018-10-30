using MovieRental.Entities.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MovieRental.Entities
{
	public class DataContext : DbContext, IDataContext
	{
		public DataContext() : base("MovieRentalDb")
		{
		}

		public DbSet<Account> Accounts { get; set; }
		public DbSet<Movie> Movies { get; set; }
		public DbSet<Kiosk> Kiosks { get; set; }
		public DbSet<KioskMovie> KioskMovies { get; set; }
		public DbSet<AccountMovie> AccountMovies { get; set; }
		public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}

        public object GetOriginalValue<TEntity>(TEntity entity, string fieldName) where TEntity : class
        {
            return Entry(entity).OriginalValues[fieldName];
        }
	}
}

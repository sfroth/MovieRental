using MovieRental.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental.Entities
{
	public class DataContext : DbContext
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
	}
}

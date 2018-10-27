using MovieRental.Entities.Models;
using System.Data.Entity;

namespace MovieRental.Entities
{
	public interface IDataContext
	{
		DbSet<Account> Accounts { get; }
		DbSet<Movie> Movies { get; }
		DbSet<Kiosk> Kiosks { get; }
		DbSet<KioskMovie> KioskMovies { get; }
		DbSet<AccountMovie> AccountMovies { get; }
		DbSet<Address> Addresses { get; }
	}
}

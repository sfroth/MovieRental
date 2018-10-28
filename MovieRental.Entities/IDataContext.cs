using MovieRental.Entities.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

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

		int SaveChanges();
		DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
	}
}

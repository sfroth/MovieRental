using MovieRental.Business.Service.Interface;
using MovieRental.Entities;
using MovieRental.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental.Business.Service
{
	public class AccountService : IAccountService
	{
		private readonly IDataContext _context;

		public AccountService(IDataContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Set account active to false
		/// </summary>
		/// <param name="id"></param>
		public void Deactivate(int id)
		{
			var account = Get(id);
			account.Active = false;
			_context.SaveChanges();
		}

	    /// <summary>
	    /// Create AccountMovie association
	    /// </summary>
	    /// <param name="accountId"></param>
	    /// <param name="movieId"></param>
	    public void Rent(int accountId, int movieId)
	    {
	        var account = Get(accountId);
	        if (account == null)
	        {
	            throw new ArgumentException("Account not found");
	        }

	        var movie = _context.Movies.FirstOrDefault(m => m.ID == movieId);
	        if (movie == null)
	        {
	            throw new ArgumentException("Movie not found");
	        }

	        var rental = new AccountMovie
	        {
	            Account = account,
	            Movie = movie,
	            RentalDate = DateTime.Now
	        };
	        _context.AccountMovies.Add(rental);
	        _context.SaveChanges();
	    }

        /// <summary>
        /// Get Account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Account Get(int id)
		{
			return _context.Accounts.FirstOrDefault(m => m.ID == id);
		}

		/// <summary>
		/// Get Account by username
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public Account Get(string username)
		{
			return _context.Accounts.FirstOrDefault(m => m.Username == username);
		}

		/// <summary>
		/// Get active Account by username and password
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public Account Get(string username, string password)
		{
			var acct = Get(username);
			// TODO: Implement password hashing here
			if (!acct.Active || acct.Password != password)
			{
				return null;
			}
			return acct;
		}

		/// <summary>
		/// Save (create or update) user account
		/// </summary>
		/// <param name="account"></param>
		public void Save(Account account)
		{
			//validate user name
			if (string.IsNullOrWhiteSpace(account.Username))
			{
				throw new ArgumentException("Username cannot be empty");
			}
			var existing = _context.Accounts.FirstOrDefault(m => m.Username == account.Username && m.ID != account.ID);
			if (existing != null)
			{
				throw new ArgumentException("Username is already taken");
			}
			if (!string.IsNullOrWhiteSpace(account.Password))
			{
				// TODO: Implement password hashing here
			}
			else if (account.ID == 0)
			{
				throw new ArgumentException("Password cannot be empty");
			}
			else
			{
				// grab existing password from database, so it doesn't get overwritten
				account.Password = (string)_context.GetOriginalValue(account, "Password");
			}
			if (account.ID == 0)
			{
				_context.Accounts.Add(account);
			}
			_context.SaveChanges();
		}

        public void Return(int accountId, int movieId)
        {
            var rental = _context.AccountMovies.FirstOrDefault(m => m.Movie.ID == movieId && m.Account.ID == accountId && m.ReturnDate == null);
            if (rental == null)
            {
                throw new ArgumentException("No active rental found");
            }

            rental.ReturnDate = DateTime.Now;

            _context.SaveChanges();
        }
    }
}

using System.Collections.Generic;
using MovieRental.Entities.Models;

namespace MovieRental.Business.Service.Interface
{
	public interface IAccountService
	{
		/// <summary>
		/// Get Account by username and password
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		Account Get(string username, string password);

		/// <summary>
		/// Get Account by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Account Get(int id);

		/// <summary>
		/// Get Account by username
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		Account Get(string username);

		/// <summary>
		/// Save (create or update) user account
		/// </summary>
		/// <param name="account"></param>
		void Save(Account account);

		/// <summary>
		/// Set account active to false
		/// </summary>
		/// <param name="id"></param>
		void Deactivate(int id);

        /// <summary>
        /// Create AccountMovie association
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="movieId"></param>
	    void Rent(int accountId, int movieId);

	    /// <summary>
	    /// Update AccountMovie association
	    /// </summary>
	    /// <param name="accountId"></param>
	    /// <param name="movieId"></param>
	    void Return(int accountId, int movieId);

        /// <summary>
        /// Get Rental History
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        IEnumerable<AccountMovie> RentalHistory(int accountId);
	}
}

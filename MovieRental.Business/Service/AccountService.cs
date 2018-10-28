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
				account.Password = (string)_context.Entry(account).OriginalValues["Password"];
			}
			if (account.ID == 0)
			{
				_context.Accounts.Add(account);
			}
			_context.SaveChanges();
		}
	}
}

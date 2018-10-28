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
		/// Get Account by username and password
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public Account Get(string username, string password)
		{
			return _context.Accounts.FirstOrDefault(m => m.Username == username && m.Password == password);
		}
	}
}

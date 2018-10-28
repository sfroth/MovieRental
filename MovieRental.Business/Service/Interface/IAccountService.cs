﻿using MovieRental.Entities.Models;

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
	}
}

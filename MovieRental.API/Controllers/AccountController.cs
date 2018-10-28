using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MovieRental.API.Controllers
{
    public class AccountController : ApiController
    {
		/// <summary>
		/// Get user account - by id or for logged in user
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[Route("account/{id?}")]
		public IHttpActionResult GetAccount(int? id)
		{
			// if id is passed in, verify user has access to that id
			// if id is not passed in, use logged in user

			//return account information
			throw new NotImplementedException();
		}

		/// <summary>
		/// Update account - by id or for logged in user
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPut]
		[Route("account/{id?}")]
		public IHttpActionResult UpdateAccount(int? id) // AccountModel account 
		{
			// if id is passed in, verify user has access to that id
			// if id is not passed in, use logged in user

			//update account
			throw new NotImplementedException();
		}

		/// <summary>
		/// Create new account
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("account")]
		public IHttpActionResult CreateAccount() // AccountModel account 
		{
			//create account
			throw new NotImplementedException();
		}
	}
}

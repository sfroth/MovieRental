using log4net;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;

namespace MovieRental.API.Controllers
{
    public class AccountController : ApiController
    {
		private readonly IAccountService _accountService;
		private readonly ILog _log = LogManager.GetLogger("MovieRental");

		public AccountController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		/// <summary>
		/// Get user account - by id or for logged in user
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[Route("account/{id?}")]
		public IHttpActionResult GetAccount(int? id = null)
		{
			// if id is passed in, verify user has access to that id
			// if id is not passed in, use logged in user
			Account account;
			try
			{
				var currentUser = _accountService.Get(User.Identity.Name);
				if (id.HasValue)
				{
					if (currentUser.Role == Account.AccountRole.User)
					{
						throw new SecurityException("Not authorized");
					}
					account = _accountService.Get(id.Value);
				}
				else
				{
					account = currentUser;
				}
			}
			catch (SecurityException)
			{
				return Unauthorized();
			}
			catch (Exception ex)
			{
				_log.Error($"Error retrieving account: {ex}");
				return InternalServerError(new ApplicationException("Error retrieving account"));
			}
			//return account information
			return Ok(account);
		}

		/// <summary>
		/// Update account - by id or for logged in user
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPut]
		[Route("account/{id?}")]
		public IHttpActionResult UpdateAccount(int? id = null) // AccountModel account 
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

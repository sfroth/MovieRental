using AutoMapper;
using log4net;
using MovieRental.API.Models;
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

		private Account _currentUser;
		private Account CurrentUser
		{
			get
			{
				if (_currentUser == null)
				{
					_currentUser = _accountService.Get(User.Identity.Name);
				}
				return _currentUser;
			}
		}

		public AccountController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		private Account GetAccountByIdOrCurrent(int? id)
		{
			// if id is passed in, verify user has access to that id
			// if id is not passed in, use logged in user
			if (id.HasValue)
			{
				if (CurrentUser.Role == Account.AccountRole.User)
				{
					throw new SecurityException("Not authorized");
				}
				return _accountService.Get(id.Value);
			}
			else
			{
				return CurrentUser;
			}
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
				account = GetAccountByIdOrCurrent(id);
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
		public IHttpActionResult UpdateAccount(AccountModel account, int? id = null)
		{
			// if id is passed in, verify user has access to that id
			// if id is not passed in, use logged in user
			Account dbAccount;
			try
			{
				dbAccount = GetAccountByIdOrCurrent(id);

				//validate role
				if (string.IsNullOrWhiteSpace(account.UserRole))
				{
					account.UserRole = dbAccount.UserRole;
				}
				if (account.UserRole != dbAccount.UserRole && CurrentUser.Role == Account.AccountRole.User)
				{
					throw new SecurityException("Not authorized");
				}
				else if (string.IsNullOrEmpty(account.UserRole))
				{
					account.UserRole = dbAccount.UserRole;
				}

				//update account
				Mapper.Map(account, dbAccount);
				_accountService.Save(dbAccount);
			}
			catch (SecurityException)
			{
				return Unauthorized();
			}
			catch (ArgumentException ex)
			{
				return InternalServerError(new ApplicationException(ex.Message));
			}
			catch (Exception ex)
			{
				_log.Error($"Error updating account: {ex}");
				return InternalServerError(new ApplicationException("Error updating account"));
			}
			//return account information
			return Ok(dbAccount);
		}

		/// <summary>
		/// Create new account
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("account")]
		public IHttpActionResult CreateAccount(AccountModel account)
		{
			//create account
			Account dbAccount;
			try
			{
				dbAccount = Mapper.Map<Account>(account);
				dbAccount.Active = true;
				_accountService.Save(dbAccount);
			}
			catch (ArgumentException ex)
			{
				return InternalServerError(new ApplicationException(ex.Message));
			}
			catch (Exception ex)
			{
				_log.Error($"Error updating account: {ex}");
				return InternalServerError(new ApplicationException("Error creating account"));
			}
			//return account information
			return Ok(dbAccount);
		}

		/// <summary>
		/// Delete account
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpDelete]
		[Route("account/{id?}")]
		public IHttpActionResult DeleteAccount(int? id = null)
		{
			Account account;
			try
			{
				account = GetAccountByIdOrCurrent(id);
				// deactivate account
				_accountService.Deactivate(account.ID);
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
			return Ok(account);
		}
	}
}

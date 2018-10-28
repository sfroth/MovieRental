using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using MovieRental.Business.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MovieRental.API.Providers
{
	public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
		private readonly IAccountService _accountService;

		public AuthorizationServerProvider(IAccountService accountService)
		{
			_accountService = accountService;
		}

		public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated(); // 
		}

		public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
		{

			// Change authentication ticket for refresh token requests  
			var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
			newIdentity.AddClaim(new Claim("newClaim", "newValue"));

			var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
			context.Validated(newTicket);

			return Task.FromResult<object>(null);

		}


		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{

			var identity = new ClaimsIdentity(context.Options.AuthenticationType);
			var account = _accountService.Get(context.UserName, context.Password);

			//Authenticate the user credentials
			if (account != null)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, account.UserRole));
				identity.AddClaim(new Claim("username", context.UserName));
				identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
				context.Validated(identity);
			}
			else
			{
				context.SetError("invalid_grant", "Provided username and password is incorrect");
				return;
			}
		}
	}
}
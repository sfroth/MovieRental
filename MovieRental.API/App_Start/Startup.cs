using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using MovieRental.API.Providers;
using Owin;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(MovieRental.API.Startup))]
namespace MovieRental.API
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
			//enable cors origin requests
			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

			//System.Web.Mvc.DependencyResolver.Current.GetService(typeof(AuthorizationServerProvider)) as AuthorizationServerProvider
			//var myProvider = new AuthorizationServerProvider();
			OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
			{
				AllowInsecureHttp = true,
				TokenEndpointPath = new PathString("/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
				Provider = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(AuthorizationServerProvider)) as AuthorizationServerProvider,
				//RefreshTokenProvider = new RefreshTokenProvider()
			};
			app.UseOAuthAuthorizationServer(options);
			app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

			HttpConfiguration config = new HttpConfiguration();
			WebApiConfig.Register(config);
		}
	}
}
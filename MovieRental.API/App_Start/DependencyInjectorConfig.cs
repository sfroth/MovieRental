using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using MovieRental.API.Providers;
using MovieRental.Business;
using MovieRental.Business.Service;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

namespace MovieRental.API
{
	public class DependencyInjectorConfig
	{
		public static void RegisterTypes()
		{
			var builder = new ContainerBuilder();

			builder.RegisterControllers(Assembly.GetExecutingAssembly());
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			builder.RegisterType<AuthorizationServerProvider>();

			builder.RegisterType<DataContext>().As<IDataContext>().InstancePerRequest();
			builder.RegisterType<MovieService>().As<IMovieService>();
			builder.RegisterType<AccountService>().As<IAccountService>();
			builder.RegisterType<KioskService>().As<IKioskService>();
			builder.RegisterType<CacheService>().As<ICacheService>();
			builder.RegisterType<StackExchangeRedisCacheClient>().As<ICacheClient>().WithParameters(new[] { new NamedParameter("serializer", new NewtonsoftSerializer()), new NamedParameter("connectionString", Config.CacheHost) });

			var container = builder.Build();
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}
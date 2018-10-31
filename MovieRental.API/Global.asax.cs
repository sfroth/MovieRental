using System.Data.Entity.SqlServer;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace MovieRental.API
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			DependencyInjectorConfig.RegisterTypes();
			AutoMapperConfig.Configure();

		    SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
		    SqlProviderServices.SqlServerTypesAssemblyName =
		        "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        }
    }
}

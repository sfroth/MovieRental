using System.Configuration;

namespace MovieRental.Business
{
	public static class Config
	{
		public static string CacheHost { get { return ConfigurationManager.AppSettings["CacheHost"]; } }
	}
}

using log4net;
using MovieRental.Business.Service.Interface;
using StackExchange.Redis.Extensions.Core;
using System;

namespace MovieRental.Business.Service
{
	public class CacheService : ICacheService
	{
		private readonly ICacheClient _client;

		private readonly ILog _log = LogManager.GetLogger("MovieRental");

		public CacheService(ICacheClient client)
		{
			if (client == null)
			{
				CacheEnabled = false;
			}
			else
			{
				CacheEnabled = true;
				_client = client;
			}
		}

		/// <summary>
		/// Is Cache enabled?
		/// </summary>
		public bool CacheEnabled { get; private set; }

		/// <summary>
		/// Get item from cache by key
		/// </summary>
		/// <typeparam name="T">Type of object in cache</typeparam>
		/// <param name="key">cache key</param>
		/// <returns></returns>
		public T Get<T>(string key)
		{
			if (!CacheEnabled)
			{
				return default(T);
			}
			try
			{
				return _client.Get<T>(key);
			}
			catch (Exception ex)
			{
				//don't let cache failures kill the whole app. best to log here, and continue on
				_log.Error(string.Format("Error reading from cache: {0}", ex));
				return default(T);
			}
		}

		/// <summary>
		/// Save item to cache
		/// </summary>
		/// <typeparam name="T">Type of object in cache</typeparam>
		/// <param name="key">cache key</param>
		/// <param name="value">object to save</param>
		/// <param name="expiresIn">Length of time object should live in cache</param>
		/// <returns>Boolean indicating whether save was successful</returns>
		public bool Set<T>(string key, T value, TimeSpan expiresIn)
		{
			if (!CacheEnabled)
			{
				return false;
			}
			try
			{
				if (value != null)
				{
					_client.Add(key, value, expiresIn);
					return true;
				}
				else
				{
					return Delete(key);
				}
			}
			catch (Exception ex)
			{
				//don't let cache failures kill the whole app. best to log here, and continue on
				_log.Error(string.Format("Error setting cache: {0}", ex));
				return false;
			}
		}

		/// <summary>
		/// Delete item from cache by key
		/// </summary>
		/// <param name="key">cache key</param>
		/// <returns></returns>
		public bool Delete(string key)
		{
			if (!CacheEnabled)
			{
				return false;
			}
			try
			{
				_client.Remove(key);
				return true;
			}
			catch (Exception ex)
			{
				//don't let cache failures kill the whole app. best to log here, and continue on
				_log.Error(string.Format("Error deleting from cache: {0}", ex));
				return false;
			}
		}
	}
}

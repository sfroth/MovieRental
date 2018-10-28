using System;

namespace MovieRental.Business.Service.Interface
{
	public interface ICacheService
	{
		/// <summary>
		/// Is Cache enabled?
		/// </summary>
		bool CacheEnabled { get; }

		/// <summary>
		/// Get item from cache by key
		/// </summary>
		/// <typeparam name="T">Type of object in cache</typeparam>
		/// <param name="key">cache key</param>
		/// <returns></returns>
		T Get<T>(string key);

		/// <summary>
		/// Save item to cache
		/// </summary>
		/// <typeparam name="T">Type of object in cache</typeparam>
		/// <param name="key">cache key</param>
		/// <param name="value">object to save</param>
		/// <param name="expiresIn">Length of time object should live in cache</param>
		/// <returns>Boolean indicating whether save was successful</returns>
		bool Set<T>(string key, T value, TimeSpan expiresIn);

		/// <summary>
		/// Delete item from cache by key
		/// </summary>
		/// <param name="key">cache key</param>
		/// <returns>Boolean indicating whether delete was successful</returns>
		bool Delete(string key);
	}
}

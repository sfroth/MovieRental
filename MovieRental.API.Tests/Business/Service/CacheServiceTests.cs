using NUnit.Framework;
using Moq;
using System;
using StackExchange.Redis;
using MovieRental.Business.Service.Interface;
using MovieRental.Business.Service;
using StackExchange.Redis.Extensions.Core;

namespace MovieRental.API.Tests.Business.Service
{
	[TestFixture]
	public class CacheServiceTests
	{
		private Mock<ICacheClient> _client;
		private ICacheService _cacheService;

		[SetUp]
		public void SetUp()
		{
			//var serializer = new NewtonsoftSerializer();
			//_client = new StackExchangeRedisCacheClient(serializer, Config.CacheHost);
			_client = new Mock<ICacheClient>();
			_cacheService = new CacheService(_client.Object);
		}

		[Test]
		public void SetCache()
		{
			_cacheService.Set("cachekey", "Hello", new TimeSpan(1, 0, 0, 0));
			_client.Verify(x => x.Add("cachekey", "Hello", new TimeSpan(1, 0, 0, 0)), Times.Once);
		}

		[Test]
		public void SetCacheNull()
		{
			string val = null;
			_cacheService.Set("cachekey", val, new TimeSpan(1, 0, 0, 0));
			_client.Verify(x => x.Add("cachekey", It.IsAny<string>(), new TimeSpan(1, 0, 0, 0)), Times.Never);
			_client.Verify(x => x.Remove("cachekey"), Times.Once);
		}

		[Test]
		public void GetCache()
		{
			var val = _cacheService.Get<string>("cachekey");
			_client.Verify(x => x.Get<string>("cachekey", It.IsAny<CommandFlags>()), Times.Once);
		}

		[Test]
		public void DeleteCache()
		{
			_cacheService.Delete("cachekey");
			_client.Verify(x => x.Remove("cachekey"), Times.Once);
		}

		[Test]
		public void CacheDisabled()
		{
			var service = new CacheService(null);
			Assert.IsFalse(service.CacheEnabled);
		}

		[Test]
		public void CacheDisabledSet()
		{
			var service = new CacheService(null);
			Assert.IsFalse(service.Set("cachekey", "Hello", new TimeSpan(1, 0, 0, 0)));
		}

		[Test]
		public void CacheDisabledGet()
		{
			var service = new CacheService(null);
			Assert.IsNull(service.Get<string>("cachekey"));
		}

		[Test]
		public void CacheDisabledDelete()
		{
			var service = new CacheService(null);
			Assert.IsFalse(service.Delete("cachekey"));
		}

		[Test]
		public void CacheGetError()
		{
			_client.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<CommandFlags>())).Throws(new Exception("test exception"));
			Assert.IsNull(_cacheService.Get<string>("cachekey"));
		}

		[Test]
		public void CacheSetError()
		{
			_client.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>())).Throws(new Exception("test exception"));
			Assert.IsFalse(_cacheService.Set("cachekey", "Hello", new TimeSpan(1, 0, 0, 0)));
		}

		[Test]
		public void CacheDeleteError()
		{
			_client.Setup(x => x.Remove(It.IsAny<string>())).Throws(new Exception("test exception"));
			Assert.IsFalse(_cacheService.Delete("cachekey"));
		}
	}
}
